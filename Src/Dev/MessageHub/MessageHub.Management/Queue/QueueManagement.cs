﻿using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessageHub.Management
{
    public class QueueManagement : IQueueManagement
    {
        private readonly ManagementClient _managementClient;

        public QueueManagement(ServiceBusConnection serviceBusConnection)
        {
            serviceBusConnection.Verify(nameof(serviceBusConnection)).IsNotNull();

            _managementClient = new ManagementClient(serviceBusConnection.ConnectionString);
        }

        public Task<bool> QueueExists(IWorkContext context, string queueName)
        {
            context.Verify(nameof(context)).IsNotNull();
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            return _managementClient.QueueExistsAsync(queueName, context.CancellationToken);
        }

        public async Task<QueueDefinition> UpdateQueue(IWorkContext context, QueueDefinition queueDefinition)
        {
            context.Verify(nameof(context)).IsNotNull();
            queueDefinition.Verify(nameof(queueDefinition)).IsNotNull();

            QueueDescription result = await _managementClient.UpdateQueueAsync(queueDefinition.ConvertTo(), context.CancellationToken);

            return result.ConvertTo();
        }

        public async Task<QueueDefinition> CreateQueue(IWorkContext context, QueueDefinition queueDefinition)
        {
            context.Verify(nameof(context)).IsNotNull();
            queueDefinition.Verify(nameof(queueDefinition)).IsNotNull();

            QueueDescription createdDescription = await _managementClient.CreateQueueAsync(queueDefinition.ConvertTo(), context.CancellationToken);
            return createdDescription.ConvertTo();
        }

        public async Task<QueueDefinition> GetQueue(IWorkContext context, string queueName)
        {
            context.Verify(nameof(context)).IsNotNull();
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            QueueDescription queueDescription = await _managementClient.GetQueueAsync(queueName, context.CancellationToken);
            return queueDescription.ConvertTo();
        }

        public Task DeleteQueue(IWorkContext context, string queueName)
        {
            context.Verify(nameof(context)).IsNotNull();
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            return _managementClient.DeleteQueueAsync(queueName, context.CancellationToken);
        }

        public async Task<IReadOnlyList<QueueDefinition>> Search(IWorkContext context, string queueName, int maxSize = 100)
        {
            context.Verify(nameof(context)).IsNotNull();
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            List<QueueDefinition> list = new List<QueueDefinition>();
            int windowSize = 100;
            int index = 0;

            string regPattern = "^" + Regex.Escape(queueName).Replace("\\*", ".*") + "$";
            Func<string, bool> isMatch = x => Regex.IsMatch(x, regPattern, RegexOptions.IgnoreCase);

            while (list.Count < maxSize)
            {
                IList<QueueDescription> subjects = await _managementClient.GetQueuesAsync(windowSize, index, context.CancellationToken);
                if (subjects.Count == 0) break;

                index += subjects.Count;
                list.AddRange(subjects.Where(x => isMatch(x.Path)).Select(x => x.ConvertTo()));
            }

            return list;
        }
    }
}
