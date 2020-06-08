using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Interface
{
    public interface IMessageNetSend
    {
        Task Send(IWorkContext context, NetMessage netMessage);

        Task<NetMessage> Call(IWorkContext context, NetMessage netMessage, TimeSpan? timeout = null);
    }
}
