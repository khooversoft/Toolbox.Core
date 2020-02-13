// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.MessageNet.Management;
using Khooversoft.Toolbox.Standard;

namespace MessageNet.Management.Test.RouteManagement
{
    public class QueueManagementFake : IQueueManagement
    {
        private readonly Dictionary<string, QueueDefinition> _queue = new Dictionary<string, QueueDefinition>(StringComparer.OrdinalIgnoreCase);
        private readonly ServiceBusConnection _serviceBusConnection;

        public QueueManagementFake(ServiceBusConnection serviceBusConnection)
        {
            _serviceBusConnection = serviceBusConnection;
        }

        public Task<QueueDefinition> CreateQueue(IWorkContext context, QueueDefinition queueDefinition)
        {
            queueDefinition.Verify(nameof(queueDefinition)).IsNotNull();

            if (_queue.ContainsKey(queueDefinition.QueueName!)) throw new InvalidOperationException();

            _queue.Add(queueDefinition.QueueName!, queueDefinition);
            return Task.FromResult(queueDefinition);
        }

        public Task DeleteQueue(IWorkContext context, string queueName)
        {
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            if (!_queue.ContainsKey(queueName)) throw new InvalidOperationException();

            _queue.Remove(queueName);
            return Task.CompletedTask;
        }

        public Task<QueueDefinition> GetQueue(IWorkContext context, string queueName)
        {
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            if (!_queue.ContainsKey(queueName)) throw new InvalidOperationException();

            return Task.FromResult(_queue[queueName]);
        }

        public Task<bool> QueueExists(IWorkContext context, string queueName)
        {
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            return Task.FromResult(_queue.ContainsKey(queueName));
        }

        public Task<IReadOnlyList<QueueDefinition>> Search(IWorkContext context, string queueName, int maxSize = 100)
        {
            throw new NotImplementedException();
        }

        public Task<QueueDefinition> UpdateQueue(IWorkContext context, QueueDefinition queueDefinition)
        {
            throw new NotImplementedException();
        }
    }
}
