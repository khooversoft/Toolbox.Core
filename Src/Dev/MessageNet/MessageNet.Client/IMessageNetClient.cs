using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Client
{
    public interface IMessageNetClient
    {
        Task<IMessageClient> GetMessageClient(IWorkContext context, string nodeId);

        Task RegisterReceiver(IWorkContext context, Func<Message, Task> receiver);
    }
}