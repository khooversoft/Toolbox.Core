// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<NodeHost> _logger;
        private QueueReceiver<NetMessageModel>? _receiver;

        public NodeHost(INodeHostReceiver nodeHostReceiver, IMessageRepository routeRepository, ILogger<NodeHost> logger)
        {
            nodeHostReceiver.VerifyNotNull(nameof(nodeHostReceiver));
            routeRepository.VerifyNotNull(nameof(routeRepository));

            _nodeHostReceiver = nodeHostReceiver;
            _routeRepository = routeRepository;
            _logger = logger;
        }

        public async Task Start(CancellationToken token)
        {
            _receiver.VerifyAssert(x => x == null, "Cannot run because receiver is already running");

            _logger.LogInformation($"Starting node host for {_nodeHostReceiver.QueueId}");
            _receiver = await _routeRepository.Register(_nodeHostReceiver.QueueId, token);
        }

        public Task Stop()
        {
            _receiver.VerifyAssert(x => x != null, "Cannot stop because receiver is not running");

            _logger.LogInformation($"Stopping node's host for {_nodeHostReceiver.QueueId}");
            return _receiver!.Stop();
        }
    }
}
