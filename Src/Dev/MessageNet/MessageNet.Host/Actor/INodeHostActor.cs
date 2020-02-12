using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal interface INodeHostActor : IActor
    {
        Task Run(IWorkContext context, IEnumerable<NodeHostRegistration> nodeRegistrations);

        Task Stop(IWorkContext context);
    }
}