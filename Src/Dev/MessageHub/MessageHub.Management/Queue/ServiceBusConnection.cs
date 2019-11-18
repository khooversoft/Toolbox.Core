using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public class ServiceBusConnection
    {
        public ServiceBusConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }
    }
}
