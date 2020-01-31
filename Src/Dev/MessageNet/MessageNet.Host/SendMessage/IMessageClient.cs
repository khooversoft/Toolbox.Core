using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    public interface IMessageClient
    {
        Task Close();

        void Dispose();

        Task Send(IWorkContext context, NetMessage message);

        Task RegisterForMessageCallBack(Guid messageId);
    }
}
