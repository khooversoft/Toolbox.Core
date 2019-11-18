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
        private readonly QueueRegistration _queueRegistration;
        private readonly ManagementClient _managementClient;

        public CreateQueueState(ServiceBusConnection serviceBusConnection, QueueRegistration queueRegistration)
        {
            serviceBusConnection.Verify(nameof(serviceBusConnection)).IsNotNull();
            queueRegistration.Verify(nameof(queueRegistration)).IsNotNull();

            _queueRegistration = queueRegistration;
            _managementClient = new ManagementClient(serviceBusConnection.ConnectionString);
        }

        public string Name => _queueRegistration.QueueDefinition!.QueueName!;

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            if (await Test(context)) return true;

            if (await _managementClient.QueueExistsAsync(_queueRegistration.QueueDefinition.QueueName, context.CancellationToken))
            {
                // Queue exist, so update it
                QueueDescription updateDescription = _queueRegistration.QueueDefinition.ConvertTo();
                await _managementClient.UpdateQueueAsync(updateDescription, context.CancellationToken);

                (await Test(context)).Verify().Assert<bool, InvalidOperationException>(x => x, "Test did not verify update");
                return true;
            }

            QueueDescription queueDescription = _queueRegistration.QueueDefinition.ConvertTo();
            QueueDescription createdDescription = await _managementClient.CreateQueueAsync(queueDescription, context.CancellationToken);

            return createdDescription != null;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            QueueDescription queueDescription = await _managementClient.GetQueueAsync(_queueRegistration.QueueDefinition.QueueName, context.CancellationToken);
            if (queueDescription == null) return false;

            var currentDescription = queueDescription.ConvertTo();
            return currentDescription == _queueRegistration.QueueDefinition;
        }
    }
}
