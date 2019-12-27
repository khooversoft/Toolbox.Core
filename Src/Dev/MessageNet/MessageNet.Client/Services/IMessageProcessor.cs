using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Client
{
    public interface IMessageProcessor
    {
        Task Close();
        void Dispose();
        Task Register(IWorkContext context, Func<Message, Task> receiver);
    }
}