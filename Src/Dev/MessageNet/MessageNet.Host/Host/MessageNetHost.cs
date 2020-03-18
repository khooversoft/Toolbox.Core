// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<string, MessageClient> _messageClients = new ConcurrentDictionary<string, MessageClient>(StringComparer.OrdinalIgnoreCase);
        private List<NodeHost>? _receivers;
        private readonly IReadOnlyList<INodeHostReceiver>? _nodeRegistrations;

        public MessageNetHost(IMessageNetConfig messageNetConfig, IMessageRepository messageRepository, IMessageAwaiterManager messageAwaiterManager, IEnumerable<INodeHostReceiver> nodeReceivers)
        {
            messageNetConfig.Verify(nameof(messageNetConfig)).IsNotNull();
            messageRepository.Verify(nameof(messageRepository)).IsNotNull();
            messageAwaiterManager.Verify(nameof(messageAwaiterManager)).IsNotNull();
            nodeReceivers.Verify(nameof(nodeReceivers)).IsNotNull().Assert(x => x.Count() > 0, "Node registrations are required");

            _messageNetConfig = messageNetConfig;
            _routeRepository = messageRepository;
            _awaiterManager = messageAwaiterManager;

            _nodeRegistrations = nodeReceivers
                .Select(x => new NodeHostReceiver(x.QueueId, y =>
                {
                    if (_awaiterManager.SetResult(y)) return Task.CompletedTask;
                    return x.Receiver(y);
                }))
                .ToList();

            _nodeRegistrations
                .GroupBy(x => x.QueueId.ToString().ToLower())
                .Where(x => x.Count() > 1)
                .Verify().Assert(x => x.Count() == 0, x => $"Duplicate routes have been detected: {string.Join(", ", x)}");
        }

        public async Task Start(IWorkContext context)
        {
            _receivers.Verify().Assert(x => x == null, "Host is already running");
            context.Telemetry.Info(context, "Starting message net host");

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

                await receivers
                    .ForEachAsync(async x => await x.Stop(context));
            }
        }

        public Task<IMessageClient> GetMessageClient(IWorkContext context, QueueId queueId)
        {
            queueId.Verify(nameof(queueId)).IsNotNull();

            string queueName = queueId.GetQueueName();

            if (!_messageNetConfig.Registrations.TryGetValue(queueId.Namespace, out NamespaceRegistration? namespaceRegistration))
            {
                throw new ArgumentException($"Cannot locate namespace {queueId.Namespace} in namespace registrations");
            }

            IMessageClient messageClient = _messageClients.GetOrAdd(queueName, x => new MessageClient(namespaceRegistration!.ConnectionString, queueName, _awaiterManager));
            return Task.FromResult(messageClient);
        }

        public async Task Send(IWorkContext context, NetMessage netMessage)
        {
            MessageUri messageUri = netMessage.Header.ToUri.ToMessageUri();
            IMessageClient messageClient = await GetMessageClient(context, messageUri.ToQueueId());
            await messageClient.Send(context, netMessage);
        }

        public async Task<NetMessage> Call(IWorkContext context, NetMessage netMessage, TimeSpan? timeout = null)
        {
            MessageUri messageUri = netMessage.Header.ToUri.ToMessageUri();
            IMessageClient messageClient = await GetMessageClient(context, messageUri.ToQueueId());
            return await messageClient.Call(context, netMessage, timeout);
        }

        public void Dispose()
        {
            _receivers
                .Verify()
                .Assert<List<NodeHost>?, InvalidOperationException>(x => x == null, "Host has not been stopped before dispose");
        }
    }
}
