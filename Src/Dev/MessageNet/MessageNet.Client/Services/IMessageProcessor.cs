using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Client
{
    public interface IMessageProcessor
    {
        Task Stop();
        void Dispose();
        Task Start(IWorkContext context, Func<NetMessage, Task> receiver);
    }
}