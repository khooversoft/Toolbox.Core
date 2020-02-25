using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Management
{
    public interface IMessageNetConfig
    {
        public string ServiceBusConnectionString { get; }

        public string Namespace { get; }
    }
}
