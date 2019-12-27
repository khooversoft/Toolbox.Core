using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageNet.Client
{
    public interface INameServerClient
    {
        Task<bool> Ping(IWorkContext context);
        
        Task ClearAll(IWorkContext context);

        Task<RouteLookupResponse?> Lookup(IWorkContext context, RouteLookupRequest request);

        Task<RouteRegistrationResponse> Register(IWorkContext context, RouteRegistrationRequest request);

        Task Unregister(IWorkContext context, RouteRegistrationRequest request);
    }
}