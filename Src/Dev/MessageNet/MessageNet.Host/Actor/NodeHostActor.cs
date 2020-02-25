﻿// Copyright (c) KhooverSoft. All rights reserved.
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
    internal class NodeHostActor : ActorBase, INodeHostActor
    {
        private readonly IConnectionManager _connectionManager;
        private readonly QueueId _queueId;
        private IList<NodeHostRegistration>? _nodeRegistrations;
        private MessageQueueReceiveProcessor? _messageProcessor;

        public NodeHostActor(IConnectionManager connectionManager)
        {
            connectionManager.Verify(nameof(connectionManager)).IsNotNull();

            _connectionManager = connectionManager;
            _queueId = ActorKey.ToQueueId();
        }

        public async Task Run(IWorkContext context, IEnumerable<NodeHostRegistration> nodeRegistrations)
        {
            context.Verify(nameof(context)).IsNotNull();
            nodeRegistrations.Verify().Assert(x => x.Count() > 0, "Node registration list is empty");
            _messageProcessor.Verify().Assert(x => x == null, "Node host is already running");

            _nodeRegistrations
                .All(x => x.MessageUri.NetworkId == _queueId.NetworkId && x.MessageUri.NodeId == _queueId.NodeId)
                .Verify()
                .Assert(x => x == true, "One or more node registration does not match network id and/or node id for the actor key");

            _nodeRegistrations = nodeRegistrations.ToList();

            string connectionString = await RegisterReceiverQueue(context);


            context.Telemetry.Info(context, $"Starting node host for {_queueId.ToString()}");
            _messageProcessor = new MessageQueueReceiveProcessor(connectionString, _queueId.ToString());

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

        protected override Task OnDeactivate(IWorkContext context)
        {
            return Stop(context);
        }

        private async Task<string> RegisterReceiverQueue(IWorkContext context)
        {
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
