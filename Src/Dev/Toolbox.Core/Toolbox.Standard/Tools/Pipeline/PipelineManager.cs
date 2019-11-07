using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Builds and manages an execution pipeline.  Each pipeline is passed the message, regardless
    /// of individual pipeline status
    /// </summary>
    /// <typeparam name="TContext">context</typeparam>
    /// <typeparam name="T">type of message</typeparam>
    public class PipelineManager<TContext, T> : IEnumerable<IPipeline<TContext, T>>, IPipelineManager<TContext, T>
    {
        private readonly IDictionary<string, IPipeline<TContext, T>> _routes =
            new Dictionary<string, IPipeline<TContext, T>>(StringComparer.OrdinalIgnoreCase);

        private readonly object _lock = new object();

        /// <summary>
        /// Default constructor
        /// </summary>
        public PipelineManager()
        {
        }

        /// <summary>
        /// Apply policy on all messages
        /// </summary>
        public Func<T, T>? ApplyPolicy { get; set; }

        /// <summary>
        /// Set apply policy
        /// </summary>
        /// <param name="applyPolicy"></param>
        /// <returns></returns>
        public IPipelineManager<TContext, T> SetApplyPolicy(Func<T, T> applyPolicy)
        {
            applyPolicy.Verify(nameof(applyPolicy)).IsNotNull();

            ApplyPolicy = applyPolicy;
            return this;
        }

        /// <summary>
        /// Add pipeline to manager with Guid name
        /// </summary>
        /// <param name="actions">pipeline actions</param>
        /// <returns>this</returns>
        public IPipelineManager<TContext, T> Add(IPipeline<TContext, T> actions)
        {
            Add(Guid.NewGuid().ToString(), actions);
            return this;
        }

        /// <summary>
        /// Added named pipeline
        /// </summary>
        /// <param name="name">name of pipeline</param>
        /// <param name="actions">actions</param>
        /// <returns></returns>
        public IPipelineManager<TContext, T> Add(string name, IPipeline<TContext, T> actions)
        {
            name.Verify(nameof(name)).IsNotEmpty();
            actions.Verify(nameof(actions)).IsNotNull();
            
            lock (_lock)
            {
                _routes[name] = actions;
            }

            return this;
        }

        /// <summary>
        /// Remove pipeline
        /// </summary>
        /// <param name="name">name of pipeline to remove</param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            name.Verify(nameof(name)).IsNotEmpty();

            lock (_lock)
            {
                return _routes.Remove(name);
            }
        }

        /// <summary>
        /// Add pipeline with a scope.  Calling Dispose() will remove pipeline
        /// from manager
        /// </summary>
        /// <param name="actions">pipeline actions</param>
        /// <returns>disposable class</returns>
        public IDisposable AddScope(IPipeline<TContext, T> actions)
        {
            string key = Guid.NewGuid().ToString();
            Add(key, actions);

            IDisposable? disposable = actions as IDisposable;
            if (disposable != null)
            {
                return new Scope<string>(key, x => { Remove(x); disposable.Dispose(); });
            }

            return new Scope<string>(key, x => Remove(x));
        }

        /// <summary>
        /// Post message to pipeline(s)
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="message">message to post</param>
        /// <returns>true if all pipelines execute successful, fail is not.</returns>          w
        public bool Post(TContext context, T message)
        {
            int itemFalseCount = 0;

            if (ApplyPolicy != null)
            {
                message = ApplyPolicy(message);
            }

            lock (_lock)
            {
                foreach (IPipeline<TContext, T> pipeline in _routes.Values)
                {
                    bool status = pipeline.Post(context, message);
                    if (!status)
                    {
                        itemFalseCount++;
                    }
                }
            }

            return itemFalseCount != _routes.Count;
        }

        /// <summary>
        /// Add pipeline to manager using "+" operator
        /// </summary>
        /// <param name="self">pipeline manager</param>
        /// <param name="actions">pipeline to add</param>
        /// <returns></returns>
        public static IPipelineManager<TContext, T> operator +(PipelineManager<TContext, T> self, IPipeline<TContext, T> actions)
        {
            actions.Verify(nameof(actions)).IsNotNull();

            return self.Add(actions);
        }

        /// <summary>
        /// Get a list of pipelines
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<IPipeline<TContext, T>> GetEnumerator()
        {
            lock (_lock)
            {
                return _routes.Values
                    .ToList()
                    .GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
