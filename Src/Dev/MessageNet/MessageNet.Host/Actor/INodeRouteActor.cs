using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal interface INodeRouteActor : IActor
    {
        Task<NodeRegistrationModel?> Lookup(IWorkContext context);

        Task<NodeRegistrationModel> Register(IWorkContext context);
    }
}