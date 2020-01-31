using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    public interface IMessageNetHost : IDisposable
    {
        Task Run(IWorkContext context);

        Task Stop(IWorkContext context);

        Task<NodeRegistrationModel?> LookupNode(IWorkContext context, string networkId, string nodeId);
    }
}
