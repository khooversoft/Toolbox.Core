using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    public class MessageNetHost : IMessageNetHost
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IWorkContext _workContext;
        private readonly Uri _nameServerUri;
        private readonly IConnectionManager _connectionManager;
        private readonly IEnumerable<NodeHostRegistration> _nodeRegistrations;
        private readonly INameServerClient _nameServerClient;
        private readonly AwaiterManager _awaiterManager = new AwaiterManager();
        private IActorManager _actorManager;
        private List<ReceiverHost>? _receivers;

        public MessageNetHost(IWorkContext context, Uri nameServerUri, IConnectionManager connectionManager, IEnumerable<NodeHostRegistration> nodeRegistrations)
        {
            context.Verify(nameof(context)).IsNotNull();
            nameServerUri.Verify(nameof(nameServerUri)).IsNotNull();
            connectionManager.Verify(nameof(connectionManager)).IsNotNull();
            nameServerUri.Verify(nameof(nameServerUri)).IsNotNull();
            nodeRegistrations.Verify(nameof(nodeRegistrations)).IsNotNull();

            _workContext = context;
            _nameServerUri = nameServerUri;
            _connectionManager = connectionManager;

            _nameServerClient = new NameServerClient(_nameServerUri, _httpClient);

            _actorManager = new ActorConfigurationBuilder()
                .Set(context)
                .Build()
                .Do(x => new ActorManager(x));

            _nodeRegistrations = nodeRegistrations
                .ToList()
                .Verify(nameof(nodeRegistrations)).Assert(x => x.Count > 0, "Node registrations are required")
                .Value;

            _nodeRegistrations
                .GroupBy(x => x.QueueName)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList()
                .Verify()
                .Assert(x => x.Count == 0, x => $"Duplicate {string.Join(", ", x)} queue names");

            _actorManager.Register<INodeHostActor>(x => new NodeHostActor(_connectionManager));
            _actorManager.Register<INodeRouteActor>(x => new NodeRouteActor(_nameServerClient));
        }


        public Task Run(IWorkContext context)
        {
            _receivers.Verify().IsNotNull("Host is already running");

            context.Telemetry.Info(context, "Starting Message Net Host");

            StartReceivers();

            return Task.CompletedTask;
        }

        public async Task Stop(IWorkContext context)
        {
            List<ReceiverHost>? receivers = Interlocked.Exchange(ref _receivers, null!);
            if (receivers != null)
            {
                context.Telemetry.Info(context, "Stopping Message Net Host");

                await _receivers
                    .ForEachAsync(async x => await x.Actor.Stop(_workContext));
            }
        }

        public async Task<NodeRegistrationModel?> LookupNode(IWorkContext context, string networkId, string nodeId)
        {
            networkId.Verify(nameof(networkId)).IsNotEmpty();
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();

            string queueName = networkId + "/" + nodeId;

            return await _actorManager.GetActor<INodeRouteActor>(new ActorKey(queueName))
                .Lookup(context);
        }

        public Task<IMessageClient> GetMessageClient(IWorkContext context, NodeRegistrationModel nodeRegistrationModel)
        {
            var uri = new Uri(nodeRegistrationModel!.InputQueueUri);
            string connectionString = _connectionManager.GetConnection(uri.Host);
            string queueName = uri.QueueName();

            return Task.FromResult<IMessageClient>(new MessageClient(connectionString, queueName, _awaiterManager));
        }

        public void Dispose()
        {
            _receivers
                .Verify()
                .Assert<List<ReceiverHost>?, InvalidOperationException>(x => x == null, "Host has not been stopped before dispose");

            IActorManager? actorManager = Interlocked.Exchange(ref _actorManager, null!);
            actorManager?.Dispose();
        }

        private void StartReceivers()
        {
            _receivers = _nodeRegistrations
                .Select(x => new MessageReceiveIntercept(x, _awaiterManager).Build())
                .Select(x => (NodeRegistration: x, Actor: _actorManager.GetActor<INodeHostActor>(new ActorKey(x.QueueName)), Task: (Task)null!))
                .Select(x => new ReceiverHost(x.Actor, x.Actor.Run(_workContext, x.NodeRegistration)))
                .ToList();
        }

        private struct ReceiverHost
        {
            public ReceiverHost(INodeHostActor actor, Task task)
            {
                Actor = actor;
                Task = task;
            }

            public INodeHostActor Actor { get; }

            public Task Task { get; }
        }

        private class MessageReceiveIntercept
        {
            private readonly NodeHostRegistration _nodeHostRegistration;
            private readonly AwaiterManager _awaiterManager;

            public MessageReceiveIntercept(NodeHostRegistration nodeHostRegistration, AwaiterManager awaiterManager)
            {
                _nodeHostRegistration = nodeHostRegistration;
                _awaiterManager = awaiterManager;
            }

            public NodeHostRegistration Build()
            {
                return new NodeHostRegistration(_nodeHostRegistration.NetworkId, _nodeHostRegistration.NodeId, x => Intercept(x));
            }

            private Task Intercept(NetMessage netMessage)
            {
                _awaiterManager.SetResult(netMessage);
                return _nodeHostRegistration.Receiver(netMessage);
            }
        }
    }
}
