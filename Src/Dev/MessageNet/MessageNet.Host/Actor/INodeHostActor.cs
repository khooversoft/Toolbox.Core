using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal interface INodeHostActor : IActor
    {
        Task Run(IWorkContext context, NodeHostRegistration nodeRegistration);

        Task Stop(IWorkContext context);
    }
}