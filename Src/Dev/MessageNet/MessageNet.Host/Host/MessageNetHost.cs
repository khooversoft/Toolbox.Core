// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
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
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<MessageNetHost> _logger;
        private readonly IMessageRepository _routeRepository;
        private readonly ConcurrentDictionary<string, MessageClient> _messageClients = new ConcurrentDictionary<string, MessageClient>(StringComparer.OrdinalIgnoreCase);
        private List<NodeHost>? _receivers;
        private readonly IReadOnlyList<INodeHostReceiver>? _nodeRegistrations;

        public MessageNetHost(
            IMessageNetConfig messageNetConfig,
            IMessageRepository messageRepository,
            IMessageAwaiterManager messageAwaiterManager,
            IEnumerable<INodeHostReceiver> nodeReceivers,
            ILoggerFactory loggerFactory)
        {
            messageNetConfig.VerifyNotNull(nameof(messageNetConfig));
            messageRepository.VerifyNotNull(nameof(messageRepository));
            messageAwaiterManager.VerifyNotNull(nameof(messageAwaiterManager));
            nodeReceivers.VerifyNotNull(nameof(nodeReceivers)).VerifyAssert(x => x.Count() > 0, "Node registrations are required");

            _messageNetConfig = messageNetConfig;
            _routeRepository = messageRepository;
            _awaiterManager = messageAwaiterManager;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<MessageNetHost>();

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
                .VerifyAssert(x => x.Count() == 0, x => $"Duplicate routes have been detected: {string.Join(", ", x)}");
        }

        public async Task Start(CancellationToken token)
        {
            _receivers.VerifyAssert(x => x == null, "Host is already running");
            _logger.LogInformation("Starting message net host");

            _receivers = _nodeRegistrations
                .Select(x => new NodeHost(x, _routeRepository, _loggerFactory.CreateLogger<NodeHost>()))
                .ToList();

            await _receivers
                .ForEachAsync(x => x.Start(token));
        }

        public async Task Stop()
        {
            List<NodeHost>? receivers = Interlocked.Exchange(ref _receivers, null!);
            if (receivers != null)
            {
                _logger.LogInformation("Starting message net host");

                await receivers
                    .ForEachAsync(async x => await x.Stop());
            }
        }

        public Task<IMessageClient> GetMessageClient(QueueId queueId)
        {
            queueId.VerifyNotNull(nameof(queueId));

            string queueName = queueId.GetQueueName();

            if (!_messageNetConfig.Registrations.TryGetValue(queueId.Namespace, out NamespaceRegistration? namespaceRegistration))
            {
                throw new ArgumentException($"Cannot locate namespace {queueId.Namespace} in namespace registrations");
            }

            IMessageClient messageClient = _messageClients.GetOrAdd(
                queueName,
                x => new MessageClient(namespaceRegistration!.ConnectionString, queueName, _awaiterManager, _loggerFactory.CreateLogger<MessageClient>()));

            return Task.FromResult(messageClient);
        }

        public async Task Send(NetMessage netMessage)
        {
            MessageUri messageUri = netMessage.Header.ToUri.ToMessageUri();
            IMessageClient messageClient = await GetMessageClient(messageUri.ToQueueId());
            await messageClient.Send(netMessage);
        }

        public async Task<NetMessage> Call(NetMessage netMessage, TimeSpan? timeout = null)
        {
            MessageUri messageUri = netMessage.Header.ToUri.ToMessageUri();
            IMessageClient messageClient = await GetMessageClient(messageUri.ToQueueId());
            return await messageClient.Call(netMessage, timeout);
        }

        public void Dispose()
        {
            _receivers.VerifyAssert<List<NodeHost>?, InvalidOperationException>(x => x == null, _ => "Host has not been stopped before dispose");
        }
    }
}
