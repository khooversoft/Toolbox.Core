using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public class QueueRegistration
    {

        public QueueRegistration(string serviceHubNamespace, string nodeId, QueueDefinition queueDefinition)
        {
            serviceHubNamespace.Verify(nameof(serviceHubNamespace)).IsNotEmpty();
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();
            queueDefinition.Verify(nameof(queueDefinition)).IsNotNull();

            ServiceHubNamespace = serviceHubNamespace;
            NodeId = nodeId;
            QueueDefinition = queueDefinition;
        }

        public string ServiceHubNamespace { get; }

        public string NodeId { get; }

        public QueueDefinition QueueDefinition { get; }
    }
}
