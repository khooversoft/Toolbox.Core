// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Graph
{
    public abstract class JobBase<TKey> : IJob, IGraphNode<TKey>, IDisposable
    {
        private Task? _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public JobBase(TKey key)
        {
            Key = key;
        }

        public TKey Key { get; set; }

        public bool Active { get; protected set; } = true;

        /// <summary>
        /// This method is called when the <see cref="IJobService"/> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="stoppingToken">Triggered when <see cref="IJobService.Stop(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        protected abstract Task Execute(IWorkContext context, CancellationToken stoppingToken);

        /// <summary>
        /// Get execution result, normally used after the job is completed
        /// </summary>
        public abstract IJobResult GetResult(IWorkContext context);

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public virtual Task Start(IWorkContext context)
        {
            // Store the task we're executing
            _executingTask = Execute(context, _stoppingCts.Token);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public virtual async Task Stop(IWorkContext context)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, context.CancellationToken));
            }
        }

        public virtual void Dispose() => _stoppingCts.Cancel();
    }
}
