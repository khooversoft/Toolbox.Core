using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public class PipelineBuilder<T>
    {
        private readonly IList<PipelineItem<T>> _pipelineItems = new List<PipelineItem<T>>();

        public PipelineBuilder() { }

        public PipelineBuilder<T> Use(Func<IWorkContext, T, Task> middleware)
        {
            _pipelineItems.Add(new PipelineItem<T>(x => true, async (context, message, next) =>
            {
                await middleware(context, message);
                await next(context, message);
            }));

            return this;
        }

        public PipelineBuilder<T> Use(Func<IWorkContext, T, Func<IWorkContext, T, Task>, Task> middleware)
        {
            _pipelineItems.Add(new PipelineItem<T>(x => true, middleware));
            return this;
        }

        public PipelineBuilder<T> Map(Func<T, bool> predicate, Func<IWorkContext, T, Task> middleware)
        {
            _pipelineItems.Add(new PipelineItem<T>(predicate, (context, message, next) =>
            {
                return middleware(context, message);
            }));

            return this;
        }

        public PipelineBuilder<T> Map(Func<T, bool> predicate, Func<IWorkContext, T, Func<IWorkContext, T, Task>, Task> middleware)
        {
            _pipelineItems.Add(new PipelineItem<T>(predicate, middleware));
            return this;
        }

        public Func<IWorkContext, T, Task> Build()
        {
            _pipelineItems.Count.Verify().Assert(x => x > 0, "Empty list");

            Func<IWorkContext, T, Task> pipeline = (context, message) =>
            {
                const string errorMsg = "End of pipeline has been reached";
                context.Telemetry.Error(context, errorMsg);
                throw new InvalidOperationException(errorMsg);
            };

            foreach (var item in _pipelineItems.Reverse())
            {
                Func<IWorkContext, T, Task> nextItem = pipeline;
                pipeline = (context, message) => item.Invoke(context, message, nextItem);
            }

            return pipeline;
        }
    }
}
