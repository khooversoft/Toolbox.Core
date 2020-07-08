using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Run
{
    public interface IActivity : IGraphNode<string>, IActivityCommon
    {
        string Name { get; }

        IProperty Properties { get; }

        IActivity WithName(string name);

        Task Run(IRunContext runContext);
    }
}
