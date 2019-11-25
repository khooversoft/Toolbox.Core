using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.Toolbox.Standard;

namespace MessageHub.Management
{
    public interface IBlobRepository
    {
        Task CreateContainer(IWorkContext context);

        Task Delete(IWorkContext context, string path);

        Task DeleteContainer(IWorkContext context);

        Task<string> Get(IWorkContext context, string path);

        Task<IReadOnlyList<string>> List(IWorkContext context);

        Task Set(IWorkContext context, string path, string data);
    }
}