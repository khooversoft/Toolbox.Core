using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Khooversoft.Toolbox.Standard;

namespace MessageNet.Host
{
    internal class ConnectionManager : ConcurrentDictionary<string, ConnectionRegistration>, IConnectionManager
    {
        public ConnectionManager()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public ConnectionManager Add(params ConnectionRegistration[] connectionRegistrations)
        {
            connectionRegistrations
                .ForEach(x => this[x.NetworkId] = x);

            return this;
        }

        public string GetConnection(string networkId) => this[networkId].ConnectionString;
    }
}
