using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public interface IBlobRepository
    {
        Task CreateContainer(IWorkContext context);

        Task Delete(IWorkContext context, string path);

        Task DeleteContainer(IWorkContext context);

        Task<string?> Get(IWorkContext context, string path);

        Task<IReadOnlyList<string>> List(IWorkContext context, string search);

        Task Set(IWorkContext context, string path, string data);

        Task ClearAll(IWorkContext context);
    }
}
