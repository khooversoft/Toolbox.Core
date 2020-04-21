using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Graph;
using System;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Run
{
    public interface IControlFlow : IGraphEdge<string>, IActivityCommon
    {
        string FromActivityName { get; }

        string ToActivityName { get; }

        Task<bool> IsValid(IWorkContext context, IRunContext runContext);
    }
}