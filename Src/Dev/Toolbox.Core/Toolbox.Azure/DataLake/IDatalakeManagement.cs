using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public interface IDatalakeManagement
    {
        Task Create(string name, CancellationToken token);
        Task CreateIfNotExist(string name, CancellationToken token);
        Task Delete(string name, CancellationToken token);
        Task DeleteIfExist(string name, CancellationToken token);
        Task<IReadOnlyList<string>> List(CancellationToken token);
    }
}