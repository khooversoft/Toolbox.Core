using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System.Threading.Tasks;

namespace MessageHub.Management
{
    public interface INodeRegistrationActor : IActor
    {
        Task Set(IWorkContext context, NodeRegistrationModel nodeRegistrationModel);

        Task Remove(IWorkContext context);

        Task<NodeRegistrationModel?> Get(IWorkContext context);

    }
}
