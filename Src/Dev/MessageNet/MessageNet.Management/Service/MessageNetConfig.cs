using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Management
{
    public class MessageNetConfig : IMessageNetConfig
    {
        public MessageNetConfig(string serviceBusConnectionString, string nameSpace)
        {
            serviceBusConnectionString.Verify(nameof(serviceBusConnectionString)).IsNotEmpty();
            nameSpace.Verify(nameof(nameSpace)).IsNotEmpty();

            ServiceBusConnectionString = serviceBusConnectionString;
            Namespace = nameSpace;
        }

        public string ServiceBusConnectionString { get; }

        public string Namespace { get; }
    }
}
