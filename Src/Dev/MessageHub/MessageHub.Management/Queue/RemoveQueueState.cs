using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageHub.Management
{
    /// <summary>
    /// Verify queue existence and/or remove it.
    /// </summary>
    public class RemoveQueueState : IStateItem
    {
        private readonly string _nodeId;
        private readonly ManagementClient _managementClient;

        public RemoveQueueState(ServiceBusConnection serviceBusConnection, string nodeId)
        {
            serviceBusConnection.Verify(nameof(serviceBusConnection)).IsNotNull();
            nodeId.Verify(nameof(nodeId)).IsNotNull();

            _nodeId = nodeId;
            _managementClient = new ManagementClient(serviceBusConnection.ConnectionString);
        }

        public string Name => _nodeId;

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            if (await Test(context)) return true;

            await _managementClient.DeleteQueueAsync(_nodeId, context.CancellationToken);
            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            QueueDescription queueDescription = await _managementClient.GetQueueAsync(_nodeId, context.CancellationToken);

            return queueDescription == null ? true : false;
        }
    }
}
