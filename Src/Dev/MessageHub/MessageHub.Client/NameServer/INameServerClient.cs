using System.Threading.Tasks;
using Khooversoft.MessageHub.Interface;
using Khooversoft.Toolbox.Standard;

namespace MessageHub.Client
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