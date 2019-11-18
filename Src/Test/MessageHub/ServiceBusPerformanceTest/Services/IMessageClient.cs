using System.Threading.Tasks;
using Khooversoft.Toolbox.Standard;

namespace ServiceBusPerformanceTest
{
    internal interface IMessageClient
    {
        Task Close();
        void Dispose();
        Task Send(IWorkContext context, string message);
    }
}