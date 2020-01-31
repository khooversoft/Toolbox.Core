using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal class NodeHostActor : ActorBase, INodeHostActor
    {
        private readonly IConnectionManager _connectionManager;
        private NodeHostRegistration? _nodeRegistration;
        private MessageQueueReceiveProcessor? _messageProcessor;

        public NodeHostActor(IConnectionManager connectionManager)
        {
            connectionManager.Verify(nameof(connectionManager)).IsNotNull();

            _connectionManager = connectionManager;
        }

        public async Task Run(IWorkContext context, NodeHostRegistration nodeRegistration)
        {
            context.Verify(nameof(context)).IsNotNull();
            nodeRegistration.Verify(nameof(nodeRegistration)).IsNotNull();
            _messageProcessor.Verify().Assert(x => x == null, "Node host is already running");

            _nodeRegistration = nodeRegistration;

            context.Telemetry.Info(context, $"Register node's host for {_nodeRegistration.QueueName}");
            NodeRegistrationModel nodeRegistrationModel = await RegisterNode(context);

            string connectionString = _connectionManager.GetConnection(_nodeRegistration.NetworkId);

            context.Telemetry.Info(context, $"Starting node host for {_nodeRegistration.QueueName}");
            _messageProcessor = new MessageQueueReceiveProcessor(connectionString, _nodeRegistration.QueueName);
            await _messageProcessor.Start(context, _nodeRegistration.Receiver);
        }

        public async Task Stop(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();

            MessageQueueReceiveProcessor? messageQueueReceiveProcessor = Interlocked.Exchange(ref _messageProcessor, null!);
            if (messageQueueReceiveProcessor == null) return;

            context.Telemetry.Info(context, $"Stopping node's host for {_nodeRegistration!.QueueName}");
            await messageQueueReceiveProcessor!.Stop();
        }

        private async Task<NodeRegistrationModel> RegisterNode(IWorkContext context)
        {
            return await ActorManager.GetActor<INodeRouteActor>(_nodeRegistration!.QueueName).Register(context);
        }

        protected override Task OnDeactivate(IWorkContext context)
        {
            return Stop(context);
        }
    }
}
