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
        private readonly IMessageNetConfig _messageNetConfig;
        private readonly IMessageAwaiterManager _awaiterManager;
        private readonly IMessageRepository _routeRepository;
        private List<NodeHost>? _receivers;
        private IEnumerable<NodeHostRegistration>? _nodeRegistrations;

        public MessageNetHost(IMessageNetConfig messageNetConfig, IMessageRepository messageRepository, IMessageAwaiterManager messageAwaiterManager)
        {
            messageNetConfig.Verify(nameof(messageNetConfig)).IsNotNull();
            messageRepository.Verify(nameof(messageRepository)).IsNotNull();
            messageAwaiterManager.Verify(nameof(messageAwaiterManager)).IsNotNull();

            _messageNetConfig = messageNetConfig;
            _routeRepository = messageRepository;
            _awaiterManager = messageAwaiterManager;
        }

        public async Task Start(IWorkContext context, IEnumerable<NodeHostRegistration> nodeRegistrations)
        {
            _receivers.Verify().Assert(x => x == null, "Host is already running");
            nodeRegistrations.Verify(nameof(nodeRegistrations)).IsNotNull();
            context.Telemetry.Info(context, "Starting message net host");

            _nodeRegistrations = nodeRegistrations
                .ToList()
                .Verify(nameof(nodeRegistrations)).Assert(x => x.Count > 0, "Node registrations are required")
                .Value;

            _nodeRegistrations
                .GroupBy(x => x.QueueId.ToString().ToLower())
                .Where(x => x.Count() > 1)
                .Verify().Assert(x => x.Count() == 0, x => $"Duplicate routes have been detected: {string.Join(", ", x)}");

            _receivers = _nodeRegistrations
                .Select(x => new NodeHost(x, _routeRepository))
                .ToList();

            await _receivers
                .ForEachAsync(x => x.Start(context));
        }

        public async Task Stop(IWorkContext context)
        {
            List<NodeHost>? receivers = Interlocked.Exchange(ref _receivers, null!);
            if (receivers != null)
            {
                context.Telemetry.Info(context, "Starting message net host");

                await _receivers
                    .ForEachAsync(async x => await x.Stop(context));
            }
        }

        public Task<IMessageClient> GetMessageClient(IWorkContext context, QueueId queueId)
        {
            queueId.Verify(nameof(queueId)).IsNotNull();

            if(_messageNetConfig.Registrations.TryGetValue(queueId.Namespace, out NamespaceRegistration? namespaceRegistration))
            {
                return Task.FromResult<IMessageClient>(new MessageClient(namespaceRegistration!.ConnectionString, queueId.GetQueueName(), _awaiterManager));
            }

            throw new ArgumentException($"Cannot locate namespace {queueId.Namespace} in namespace registrations");
        }

        public void Dispose()
        {
            _receivers
                .Verify()
                .Assert<List<NodeHost>?, InvalidOperationException>(x => x == null, "Host has not been stopped before dispose");
        }
    }
}
