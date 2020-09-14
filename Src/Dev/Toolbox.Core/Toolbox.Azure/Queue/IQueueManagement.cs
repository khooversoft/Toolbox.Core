// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public interface IQueueManagement
    {
        string ConnectionString { get; }

        Task<QueueDefinition> Create(QueueDefinition queueDefinition, CancellationToken token);
        Task Delete(string queueName, CancellationToken token);
        Task<QueueDefinition> GetDefinition(string queueName, CancellationToken token);
        Task<bool> Exist(string queueName, CancellationToken token);
        Task<IReadOnlyList<QueueDefinition>> Search(CancellationToken token, string? search = null, int maxSize = 100);
        Task<QueueDefinition> Update(QueueDefinition queueDefinition, CancellationToken token);
        Task<QueueDefinition> CreateIfNotExist(QueueDefinition queueDefinition, CancellationToken token);
        Task DeleteIfExist(string queueName, CancellationToken token);
    }
}