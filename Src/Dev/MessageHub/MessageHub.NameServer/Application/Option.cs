using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageHub.NameServer
{
    public class Option
    {
        public string ServiceBusConnection { get; set; } = null!;

        public BlobRepositoryDetails BlobRepository { get; set; } = null!;

        public void Verify()
        {
            ServiceBusConnection!.Verify(nameof(ServiceBusConnection)).IsNotEmpty();
            BlobRepository!.Verify(nameof(BlobRepository)).IsNotNull().Value.Verify();
        }
    }
}
