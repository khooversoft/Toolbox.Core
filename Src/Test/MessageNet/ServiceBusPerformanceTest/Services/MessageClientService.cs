using System;
using System.Collections.Generic;
using System.Text;
using Khooversoft.MessageNet.Interface;
using Khooversoft.MessageNet.Client;

namespace ServiceBusPerformanceTest
{
    internal class MessageClientService : MessageClient
    {
        public MessageClientService(IOption option)
            : base(option.ServiceBusConnectionString!, option.QueueName!)
        {
        }
    }
}
