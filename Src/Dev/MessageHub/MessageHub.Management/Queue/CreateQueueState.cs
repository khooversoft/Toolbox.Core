using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageHub.Management
{
    /// <summary>
    /// Verify queue does exist or create it
    /// </summary>
    public class CreateQueueState : IStateItem
    {
        private readonly QueueDefinition _queueDefinition;
        private readonly IQueueManagement _managementClient;

        public CreateQueueState(IQueueManagement queueManagement, QueueDefinition queueDefinition)
        {
            queueManagement.Verify(nameof(queueManagement)).IsNotNull();
            queueDefinition.Verify(nameof(queueDefinition)).IsNotNull();
            queueDefinition.QueueName.Verify(nameof(queueDefinition.QueueName)).IsNotNull();

            _queueDefinition = queueDefinition;
            _managementClient = queueManagement;
        }

        public string Name => _queueDefinition!.QueueName!;

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            if (await Test(context)) return true;

            if (await _managementClient.QueueExists(context, _queueDefinition.QueueName!))
            {
                // Queue exist, so update it
                await _managementClient.UpdateQueue(context, _queueDefinition);

                (await Test(context)).Verify().Assert<bool, InvalidOperationException>(x => x, "Test did not verify update");
                return true;
            }

            await _managementClient.CreateQueue(context, _queueDefinition);
            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            if ((await _managementClient.QueueExists(context, _queueDefinition.QueueName!) == false)) return false;

            QueueDefinition subject = await _managementClient.GetQueue(context, _queueDefinition.QueueName!);
            return (_queueDefinition == subject);
        }
    }
}
