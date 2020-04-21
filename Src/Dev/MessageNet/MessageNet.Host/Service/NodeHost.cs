// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    internal class NodeHost
    {
        private readonly INodeHostReceiver _nodeHostReceiver;
        private readonly IMessageRepository _routeRepository;
        private QueueReceiver<NetMessageModel>? _receiver;

        public NodeHost(INodeHostReceiver nodeHostReceiver, IMessageRepository routeRepository)
        {
            nodeHostReceiver.VerifyNotNull(nameof(nodeHostReceiver));
            routeRepository.VerifyNotNull(nameof(routeRepository));

            _nodeHostReceiver = nodeHostReceiver;
            _routeRepository = routeRepository;
        }

        public async Task Start(IWorkContext context)
        {
            _receiver.VerifyAssert(x => x == null, "Cannot run because receiver is already running");
            context.VerifyNotNull(nameof(context));

            context.Telemetry.Info(context, $"Starting node host for {_nodeHostReceiver.QueueId}");
            _receiver = await _routeRepository.Register(context, _nodeHostReceiver.QueueId);

            await _receiver.Start(context, x => _nodeHostReceiver.Receiver(x.ConvertTo()));
        }

        public Task Stop(IWorkContext context)
        {
            _receiver.VerifyAssert(x => x != null, "Cannot stop because receiver is not running");
            context.VerifyNotNull(nameof(context));

            context.Telemetry.Info(context, $"Stopping node's host for {_nodeHostReceiver.QueueId}");
            return _receiver!.Stop();
        }
    }
}
