using System;
using System.Threading.Tasks;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;

namespace ServiceBusPerformanceTest
{
    internal interface IMessageProcessor
    {
        Task Close();
        Task Register(IWorkContext context, Func<Message, Task> receiver);
    }
}