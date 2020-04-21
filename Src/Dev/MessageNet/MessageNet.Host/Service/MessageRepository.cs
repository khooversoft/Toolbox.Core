﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IReadOnlyDictionary<string, IQueueManagement> _registrations;
        private static readonly RetryPolicy _retryPolicy = new RetryPolicy(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1));

        public MessageRepository(IMessageNetConfig messageNetConfig)
        {
            _registrations = messageNetConfig.Registrations.ToDictionary(
                x => x.Value.Namespace,
                x => (IQueueManagement)new QueueManagement(x.Value.ConnectionString),
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Register Node by NodeId
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        public async Task<QueueReceiver<NetMessageModel>> Register(IWorkContext context, QueueId queueId)
        {
            string queueName = queueId.GetQueueName();

            if (!_registrations.TryGetValue(queueId.Namespace, out IQueueManagement? queueManagement)) throw new ArgumentException($"Queue's namespace {queueId.Namespace} is not registered");

            QueueDefinition queueDefinition = new QueueDefinition(queueName);

            await new StateManagerBuilder()
                .Add(new CreateQueueState(queueManagement, queueDefinition))
                .AddPolicy(_retryPolicy)
                .Build()
                .Set(context);

            return new QueueReceiver<NetMessageModel>(queueManagement.ConnectionString, queueId.GetQueueName());
        }

        /// <summary>
        /// Unregistered route
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="nodeId">node id</param>
        /// <returns>task</returns>
        public Task Unregister(IWorkContext context, QueueId queueId)
        {
            string queueName = queueId.ToString();

            if (!_registrations.TryGetValue(queueId.Namespace, out IQueueManagement? queueManagement)) throw new ArgumentException($"Queue's namespace {queueId.Namespace} is not registered");

            return new StateManagerBuilder()
                .Add(new RemoveQueueState(queueManagement, queueId.GetQueueName()))
                .AddPolicy(_retryPolicy)
                .Build()
                .Set(context);
        }

        /// <summary>
        /// Search for node registration
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<QueueDefinition>> Search(IWorkContext context, string search)
        {
            search.VerifyNotNull(nameof(search));

            List<Task<IReadOnlyList<QueueDefinition>>> tasks = _registrations
                .Select(x => x.Value.Search(context, search))
                .ToList();

            IReadOnlyList<QueueDefinition>[] results = await Task.WhenAll(tasks);

            return results
                .SelectMany(x => x)
                .ToList();
        }
    }
}
