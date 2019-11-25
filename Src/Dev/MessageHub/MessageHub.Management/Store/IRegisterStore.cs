using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageHub.Management
{
    public interface IRegisterStore
    {
        Task Set(IWorkContext context, string path, NodeRegistrationModel nodeRegistrationModel);

        Task Remove(IWorkContext context, string path);

        Task<NodeRegistrationModel?> Get(IWorkContext context, string path);

        Task<IReadOnlyList<NodeRegistrationModel>> List(IWorkContext context);
    }
}