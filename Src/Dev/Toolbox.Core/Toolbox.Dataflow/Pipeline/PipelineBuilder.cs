// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Dataflow
{
    public class PipelineBuilder<T>
    {
        private readonly IList<PipelineItem<T>> _pipelineItems = new List<PipelineItem<T>>();

        public PipelineBuilder() { }

        public PipelineBuilder<T> Use(Func<T, Task> middleware)
        {
            _pipelineItems.Add(new PipelineItem<T>(x => true, async (message, next) =>
            {
                await middleware(message);
                await next(message);
            }));

            return this;
        }

        public PipelineBuilder<T> Use(Func<T, Func<T, Task>, Task> middleware)
        {
            _pipelineItems.Add(new PipelineItem<T>(x => true, middleware));
            return this;
        }

        public PipelineBuilder<T> Map(Func<T, bool> predicate, Func<T, Task> middleware)
        {
            _pipelineItems.Add(new PipelineItem<T>(predicate, (message, next) =>
            {
                return middleware(message);
            }));

            return this;
        }

        public PipelineBuilder<T> Map(Func<T, bool> predicate, Func<T, Func<T, Task>, Task> middleware)
        {
            _pipelineItems.Add(new PipelineItem<T>(predicate, middleware));
            return this;
        }

        public Func<T, Task> Build()
        {
            _pipelineItems.Count.VerifyAssert(x => x > 0, _ => "Empty list");

            Func<T, Task> pipeline = (message) =>
            {
                const string errorMsg = "End of pipeline has been reached";
                throw new InvalidOperationException(errorMsg);
            };

            foreach (var item in _pipelineItems.Reverse())
            {
                Func<T, Task> nextItem = pipeline;
                pipeline = (message) => item.Invoke(message, nextItem);
            }

            return pipeline;
        }
    }
}
