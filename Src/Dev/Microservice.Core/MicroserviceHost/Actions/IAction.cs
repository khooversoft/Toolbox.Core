using Khooversoft.Toolbox.Standard;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    internal interface IAction
    {
        Task Run(IWorkContext context, IExecutionContext executionContext);
    }
}