using System;
using System.Collections.Generic;
using System.Text;

namespace MessageNet.Host
{
    internal class ConnectionRegistration
    {
        public ConnectionRegistration(string networkId, string connectionString)
        {
            NetworkId = networkId;
            ConnectionString = connectionString;
        }

        public string NetworkId { get; }

        public string ConnectionString { get; }
    }
}
