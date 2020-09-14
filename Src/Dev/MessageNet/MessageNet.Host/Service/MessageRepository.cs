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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IReadOnlyDictionary<string, IQueueManagement> _registrations;
        private readonly ILoggerFactory _loggerFactory;

        //private static readonly RetryPolicy _retryPolicy = new RetryPolicy(TimeSpan.FromSeconds(10));

        public MessageRepository(IMessageNetConfig messageNetConfig, ILoggerFactory loggerFactory)
        {
            _registrations = messageNetConfig.Registrations.ToDictionary(
                x => x.Value.Namespace,
                x => (IQueueManagement)new QueueManagement(x.Value.ConnectionString, loggerFactory.CreateLogger<QueueManagement>()),
                StringComparer.OrdinalIgnoreCase);
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Register Node by NodeId
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        public async Task<QueueReceiver<NetMessageModel>> Register(QueueId queueId, CancellationToken token)
        {
            string queueName = queueId.GetQueueName();

            if (!_registrations.TryGetValue(queueId.Namespace, out IQueueManagement? queueManagement)) throw new ArgumentException($"Queue's namespace {queueId.Namespace} is not registered");

            QueueDefinition queueDefinition = new QueueDefinition(queueName);
            await queueManagement.CreateIfNotExist(queueDefinition, token);

            return new QueueReceiver<NetMessageModel>(queueManagement.ConnectionString, queueId.GetQueueName(), _loggerFactory.CreateLogger<QueueReceiver<NetMessageModel>>());
        }

        /// <summary>
        /// Unregistered route
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="nodeId">node id</param>
        /// <returns>task</returns>
        public async Task Unregister(QueueId queueId, CancellationToken token)
        {
            string queueName = queueId.ToString();

            if (!_registrations.TryGetValue(queueId.Namespace, out IQueueManagement? queueManagement)) throw new ArgumentException($"Queue's namespace {queueId.Namespace} is not registered");

            await queueManagement.DeleteIfExist(queueId.GetQueueName(), token);
        }

        /// <summary>
        /// Search for node registration
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<QueueDefinition>> Search(string search, CancellationToken token)
        {
            search.VerifyNotNull(nameof(search));

            List<Task<IReadOnlyList<QueueDefinition>>> tasks = _registrations
                .Select(x => x.Value.Search(token, search))
                .ToList();

            IReadOnlyList<QueueDefinition>[] results = await Task.WhenAll(tasks);

            return results
                .SelectMany(x => x)
                .ToList();
        }
    }
}
