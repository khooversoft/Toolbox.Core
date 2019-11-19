using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.Toolbox.Standard;

namespace MessageHub.Management
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