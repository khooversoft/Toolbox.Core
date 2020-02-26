// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    /// <summary>
    /// Message net host creates receivers and routes for each queue name which is = "networkId + "/" + "nodeId".
    /// 
    /// Messages are routed to "route" function, which receives the message for processing.
    /// 
    /// </summary>
    public class MessageNetHost : IMessageNetHost
    {
        private const string hostIdFormat = "{namespace}/{networkId}/{nodeId}";

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
                .GroupBy(x => x.QueueId.ToString(hostIdFormat))
                .Where(x => x.Count() > 1)
                .Verify().Assert(x => x.Count() == 0, x => $"Duplicate routes have been detected: {string.Join(", ", x)}");

            _actorManager.Register<INodeHost>(x => new NodeHost(_connectionManager));
            _actorManager.Register<INodeRouteActor>(x => new NodeRouteActor(_nameServerClient));
        }

        public Task Run(IWorkContext context)
        {
            _receivers.Verify().IsNotNull("Host is already running");

            context.Telemetry.Info(context, "Starting message net host");

            StartReceivers();

            return Task.CompletedTask;
        }

        public async Task Stop(IWorkContext context)
        {
            List<ReceiverHost>? receivers = Interlocked.Exchange(ref _receivers, null!);
            if (receivers != null)
            {
                context.Telemetry.Info(context, "Starting message net host");

                await _receivers
                    .ForEachAsync(async x => await x.Actor.Stop(_workContext));
            }
        }

        public async Task<NodeRegistration?> LookupNode(IWorkContext context, QueueId queueId)
        {
            queueId.Verify(nameof(queueId)).IsNotNull();

            var actorKey = queueId.ToActorKey();

            return await _actorManager.GetActor<INodeRouteActor>(actorKey)
                .Lookup(context);
        }

        public Task<IMessageClient> GetMessageClient(IWorkContext context, NodeRegistration nodeRegistration)
        {
            string connectionString = _connectionManager.GetConnection(nodeRegistration.Namespace);
            string queueName = nodeRegistration.QueueId.ToString();

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
                .GroupBy(x => x.QueueId.ToString(hostIdFormat))
                .Select(x => (NodeRegistrations: x, Actor: _actorManager.GetActor<INodeHost>(new ActorKey(x.Key))))
                .Select(x => new ReceiverHost(x.Actor, x.Actor.Run(_workContext, x.NodeRegistrations)))
                .ToList();
        }

        private struct ReceiverHost
        {
            public ReceiverHost(INodeHost actor, Task task)
            {
                Actor = actor;
                Task = task;
            }

            public INodeHost Actor { get; }

            public Task Task { get; }
        }
    }
}
