// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageHub.Management
{
    public interface IQueueManagement
    {
        Task<QueueDefinition> CreateQueue(IWorkContext context, QueueDefinition queueDefinition);

        Task DeleteQueue(IWorkContext context, string queueName);

        Task<QueueDefinition> GetQueue(IWorkContext context, string queueName);

        Task<bool> QueueExists(IWorkContext context, string queueName);

        Task<IReadOnlyList<QueueDefinition>> Search(IWorkContext context, string queueName, int maxSize = 100);

        Task<QueueDefinition> UpdateQueue(IWorkContext context, QueueDefinition queueDefinition);
    }
}