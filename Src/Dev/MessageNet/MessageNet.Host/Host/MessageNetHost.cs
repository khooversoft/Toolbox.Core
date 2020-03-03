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
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IWorkContext _workContext;
        private readonly IMessageNetConfig _messageNetConfig;
        private readonly IEnumerable<NodeHostRegistration> _nodeRegistrations;
        private readonly MessageAwaiterManager _awaiterManager = new MessageAwaiterManager();
        private readonly IMessageRepository _routeRepository;
        private List<NodeHost>? _receivers;

        public MessageNetHost(IWorkContext context, IMessageNetConfig messageNetConfig, IEnumerable<NodeHostRegistration> nodeRegistrations)
        {
            context.Verify(nameof(context)).IsNotNull();
            messageNetConfig.Verify(nameof(messageNetConfig)).IsNotNull();
            nodeRegistrations.Verify(nameof(nodeRegistrations)).IsNotNull();

            _workContext = context;
            _messageNetConfig = messageNetConfig;
            _routeRepository = new MessageRepository(_messageNetConfig);

            _nodeRegistrations = nodeRegistrations
                .ToList()
                .Verify(nameof(nodeRegistrations)).Assert(x => x.Count > 0, "Node registrations are required")
                .Value;

            _nodeRegistrations
                .GroupBy(x => x.QueueId.ToString().ToLower())
                .Where(x => x.Count() > 1)
                .Verify().Assert(x => x.Count() == 0, x => $"Duplicate routes have been detected: {string.Join(", ", x)}");
        }

        public Task Run(IWorkContext context)
        {
            _receivers.Verify().IsNotNull("Host is already running");
            context.Telemetry.Info(context, "Starting message net host");

            _receivers = _nodeRegistrations
                .Select(x => new NodeHost(x, _routeRepository))
                .ToList();

            return Task.CompletedTask;
        }

        public async Task Stop(IWorkContext context)
        {
            List<NodeHost>? receivers = Interlocked.Exchange(ref _receivers, null!);
            if (receivers != null)
            {
                context.Telemetry.Info(context, "Starting message net host");

                await _receivers
                    .ForEachAsync(async x => await x.Stop(_workContext));
            }
        }

        public Task<IMessageClient> GetMessageClient(IWorkContext context, QueueId queueId)
        {
            queueId.Verify(nameof(queueId)).IsNotNull();

            string connectionString = _messageNetConfig.Registrations[queueId.Namespace].ConnectionString;

            return Task.FromResult<IMessageClient>(new MessageClient(connectionString, queueId.GetQueueName(), _awaiterManager));
        }

        public void Dispose()
        {
            _receivers
                .Verify()
                .Assert<List<NodeHost>?, InvalidOperationException>(x => x == null, "Host has not been stopped before dispose");
        }
    }
}
