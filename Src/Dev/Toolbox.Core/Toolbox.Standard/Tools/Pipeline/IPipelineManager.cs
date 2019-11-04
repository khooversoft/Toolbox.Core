using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public interface IPipelineManager<TContext, T> : IPipeline<TContext, T>
    {
        IPipelineManager<TContext, T> Add(IPipeline<TContext, T> actions);

        IPipelineManager<TContext, T> Add(string name, IPipeline<TContext, T> actions);

        IDisposable AddScope(IPipeline<TContext, T> actions);
    }
}
