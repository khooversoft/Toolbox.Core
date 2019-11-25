using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public class ServiceBusConnection
    {
        public ServiceBusConnection(string connectionString)
        {
            connectionString.Verify(nameof(connectionString)).IsNotEmpty();

            ConnectionString = connectionString;

            // TODO: need to extract service bus name
            ServiceBusName = new ServiceBusConnectionStringBuilder(ConnectionString).Endpoint;
        }

        public string ConnectionString { get; set; }

        public string ServiceBusName { get; set; }
    }
}
