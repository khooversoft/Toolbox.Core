using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public class RouteRegistration
    {
        public RouteRegistration(string queueName, QueueRegistration queueRegistration)
        {
            queueName.Verify(nameof(queueName)).IsNotNull();
            queueRegistration.Verify(nameof(queueRegistration)).IsNotNull();

            QueueName = queueName;
            QueueRegistration = queueRegistration;
        }

        public string QueueName { get; }

        public QueueRegistration QueueRegistration { get; }
    }
}
