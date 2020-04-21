// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public class QueueManagement : IQueueManagement
    {
        private readonly ManagementClient _managementClient;

        public QueueManagement(string connectionString)
        {
            connectionString.VerifyNotEmpty(nameof(connectionString));

            _managementClient = new ManagementClient(connectionString);
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public Task<bool> QueueExists(IWorkContext context, string queueName)
        {
            context.VerifyNotNull(nameof(context));
            queueName.VerifyNotEmpty(nameof(queueName));

            return _managementClient.QueueExistsAsync(queueName, context.CancellationToken);
        }

        public async Task<QueueDefinition> UpdateQueue(IWorkContext context, QueueDefinition queueDefinition)
        {
            context.VerifyNotNull(nameof(context));
            queueDefinition.VerifyNotNull(nameof(queueDefinition));

            QueueDescription result = await _managementClient.UpdateQueueAsync(queueDefinition.ConvertTo(), context.CancellationToken);

            return result.ConvertTo();
        }

        public async Task<QueueDefinition> CreateQueue(IWorkContext context, QueueDefinition queueDefinition)
        {
            context.VerifyNotNull(nameof(context));
            queueDefinition.VerifyNotNull(nameof(queueDefinition));

            QueueDescription createdDescription = await _managementClient.CreateQueueAsync(queueDefinition.ConvertTo(), context.CancellationToken);
            return createdDescription.ConvertTo();
        }

        public async Task<QueueDefinition> GetQueue(IWorkContext context, string queueName)
        {
            context.VerifyNotNull(nameof(context));
            queueName.VerifyNotEmpty(nameof(queueName));

            QueueDescription queueDescription = await _managementClient.GetQueueAsync(queueName, context.CancellationToken);
            return queueDescription.ConvertTo();
        }

        public Task DeleteQueue(IWorkContext context, string queueName)
        {
            context.VerifyNotNull(nameof(context));
            queueName.VerifyNotEmpty(nameof(queueName));

            return _managementClient.DeleteQueueAsync(queueName, context.CancellationToken);
        }

        public async Task<IReadOnlyList<QueueDefinition>> Search(IWorkContext context, string? search = null, int maxSize = 100)
        {
            context.VerifyNotNull(nameof(context));

            List<QueueDefinition> list = new List<QueueDefinition>();
            int windowSize = 100;
            int index = 0;

            string regPattern = "^" + Regex.Escape(search ?? string.Empty).Replace("\\*", ".*") + "$";
            Func<string, bool> isMatch = x => Regex.IsMatch(x, regPattern, RegexOptions.IgnoreCase);

            while (list.Count < maxSize)
            {
                IList<QueueDescription> subjects = await _managementClient.GetQueuesAsync(windowSize, index, context.CancellationToken);
                if (subjects.Count == 0) break;

                index += subjects.Count;
                list.AddRange(subjects.Where(x => search == null || isMatch(x.Path)).Select(x => x.ConvertTo()));
            }

            return list;
        }
    }
}
