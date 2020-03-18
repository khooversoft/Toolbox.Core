using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    internal class NamespaceConnection
    {
        public string Namespace { get; set; } = null!;

        public string ConnectionString { get; set; } = null!;

        public void Verify()
        {
            Namespace.Verify(nameof(Namespace)).IsNotNull();
            ConnectionString.Verify(nameof(ConnectionString)).IsNotNull();
        }
    }
}
