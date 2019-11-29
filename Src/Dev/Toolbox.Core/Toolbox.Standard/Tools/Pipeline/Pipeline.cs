// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Delegate for executing a pipeline
    /// </summary>
    /// <typeparam name="TContext">execution context</typeparam>
    /// <typeparam name="T">message type</typeparam>
    /// <param name="context">execution context</param>
    /// <param name="message">message to process</param>
    /// <returns>true if pipeline was successful</returns>
    public delegate bool PipelineFunc<TContext, T>(TContext context, T message);

    /// <summary>
    /// Pipeline container, executes all actions until false is returned
    /// </summary>
    /// <typeparam name="TContext">execution context</typeparam>
    /// <typeparam name="T">message type</typeparam>
    public class Pipeline<TContext, T> : IReadOnlyList<PipelineFunc<TContext, T>>, IPipeline<TContext, T>
    {
        private readonly List<PipelineFunc<TContext, T>> _pipelines = new List<PipelineFunc<TContext, T>>();
        private readonly object _lock = new object();

        public Pipeline()
        {
        }

        public PipelineFunc<TContext, T> this[int index] => _pipelines[index];

        public int Count => _pipelines.Count;

        public Pipeline<TContext, T> Add(PipelineFunc<TContext, T> actor)
        {
            actor.Verify(nameof(actor)).IsNotNull();

            lock (_lock)
            {
                _pipelines.Add(actor);
            }

            return this;
        }

        public bool Post(TContext context, T message)
        {
            lock (_lock)
            {
                foreach (var item in _pipelines)
                {
                    if (!item.Invoke(context, message))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public IEnumerator<PipelineFunc<TContext, T>> GetEnumerator()
        {
            lock (_pipelines)
            {
                return _pipelines
                    .ToList()
                    .GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Pipeline<TContext, T> operator +(Pipeline<TContext, T> self, PipelineFunc<TContext, T> actor)
        {
            self.Verify(nameof(self)).IsNotNull();
            actor.Verify(nameof(actor)).IsNotNull();

            self.Add(actor);
            return self;
        }
    }
}
