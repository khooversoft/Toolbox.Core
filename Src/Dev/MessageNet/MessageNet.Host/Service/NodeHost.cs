// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    internal class NodeHost : INodeHost
    {
        private readonly IMessageNetConfig _messageNetConfig;
        private readonly NodeHostRegistration _nodeRegistration
            ;
        private readonly IRouteRepository _routeRepository;
        private MessageQueueReceiveProcessor? _messageProcessor;

        public NodeHost(IMessageNetConfig messageNetConfig, NodeHostRegistration nodeRegistration, IRouteRepository routeRepository)
        {
            messageNetConfig.Verify(nameof(messageNetConfig)).IsNotNull();
            nodeRegistration.Verify(nameof(nodeRegistration)).IsNotNull();

            _messageNetConfig = messageNetConfig;
            _nodeRegistration = nodeRegistration;
            _routeRepository = routeRepository;
        }

        public async Task Run(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            _messageProcessor.Verify().Assert(x => x == null, "Node host is already running");

            string connectionString = await RegisterReceiverQueue(context);


            context.Telemetry.Info(context, $"Starting node host for {_nodeRegistration.QueueId.ToString()}");
            _messageProcessor = new MessageQueueReceiveProcessor(connectionString, _nodeRegistration.QueueId.GetQueueName());

            Func<IWorkContext, NetMessage, Task> pipeline = CreatePipeline();
            await _messageProcessor.Start(context, x => pipeline(context, x));
        }

        public async Task Stop(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();

            MessageQueueReceiveProcessor? messageQueueReceiveProcessor = Interlocked.Exchange(ref _messageProcessor, null!);
            if (messageQueueReceiveProcessor == null) return;

            context.Telemetry.Info(context, $"Stopping node's host for {ActorKey}");
            await messageQueueReceiveProcessor!.Stop();
        }

        private async Task<string> RegisterReceiverQueue(IWorkContext context)
        {
            string connectionString = _messageNetConfig.NamespaceRegistrations[_nodeRegistration.QueueId.Namespace].ConnectionString;

            _routeRepository.Register(context, )

            context.Telemetry.Info(context, $"Register node's host for {ActorKey}");
            await ActorManager.GetActor<INodeRouteActor>(ActorKey).Register(context);

            return _connectionManager.GetConnection(_queueId.NetworkId);
        }

        private Func<IWorkContext, NetMessage, Task> CreatePipeline()
        {
            var pipelineBuilder = new PipelineBuilder<NetMessage>();

            foreach (var item in _nodeRegistrations!)
            {
                pipelineBuilder.Map(x => x.Header.ToUri.ToMessageUri().ToQueueId() == _queueId, (context, message) => item.Receiver(message));
            }

            return pipelineBuilder.Build();
        }
    }
}
