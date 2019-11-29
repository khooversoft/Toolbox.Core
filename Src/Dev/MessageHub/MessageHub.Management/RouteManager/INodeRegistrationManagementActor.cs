using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.MessageHub.Management;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageHub.Management
{
    public interface INodeRegistrationManagementActor : IActor
    {
        Task<IReadOnlyList<NodeRegistrationModel>> List(IWorkContext context, string search);
    }
}