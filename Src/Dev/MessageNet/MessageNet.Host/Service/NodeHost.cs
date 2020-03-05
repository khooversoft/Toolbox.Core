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
        private readonly NodeHostRegistration _nodeRegistration;
        private readonly IMessageRepository _routeRepository;
        private QueueReceiver<NetMessageModel>? _receiver;

        public NodeHost(NodeHostRegistration nodeRegistration, IMessageRepository routeRepository)
        {
            nodeRegistration.Verify(nameof(nodeRegistration)).IsNotNull();
            routeRepository.Verify(nameof(routeRepository)).IsNotNull();

            _nodeRegistration = nodeRegistration;
            _routeRepository = routeRepository;
        }

        public async Task Start(IWorkContext context)
        {
            _receiver.Verify().Assert(x => x == null, "Cannot run because receiver is already running");
            context.Verify(nameof(context)).IsNotNull();

            context.Telemetry.Info(context, $"Starting node host for {_nodeRegistration.QueueId}");
            _receiver = await _routeRepository.Register(context, _nodeRegistration.QueueId);

            await _receiver.Start(context, x => _nodeRegistration.Receiver(x.ConvertTo()));
        }

        public Task Stop(IWorkContext context)
        {
            _receiver.Verify().Assert(x => x != null, "Cannot stop because receiver is not running");
            context.Verify(nameof(context)).IsNotNull();

            context.Telemetry.Info(context, $"Stopping node's host for {_nodeRegistration.QueueId}");
            return _receiver!.Stop();
        }
    }
}
