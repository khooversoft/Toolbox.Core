using System;
using System.Collections.Generic;
using System.Text;
using Khooversoft.MessageHub.Interface;

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
