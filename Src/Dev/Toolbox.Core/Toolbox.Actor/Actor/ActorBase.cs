// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Base class for actors
    /// </summary>
    public abstract class ActorBase : IActorBase
    {
        private int _timerLockValue;
        private Timer? _timer;
        private readonly StringVector _tag = new StringVector(nameof(ActorBase));
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private int _running = 0;

        /// <summary>
        /// Get actor key
        /// </summary>
        public ActorKey ActorKey { get; internal set; } = ActorKey.Default;

        /// <summary>
        /// Get actor manager
        /// </summary>
        public IActorManager ActorManager { get; internal set; } = null!;

        /// <summary>
        /// Actor type, to deactivate
        /// </summary>
        public Type ActorType { get; internal set; } = null!;

        /// <summary>
        /// Actor is active (running)
        /// </summary>
        public bool Active => _running == 1;

        /// <summary>
        /// Activate actor
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        public async Task Activate(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            context = context.With(_tag);

            int currentValue = Interlocked.CompareExchange(ref _running, 1, 0);
            if (currentValue != 0)
            {
                return;
            }

            ActorManager.Configuration.ActorActivateEvent(context, ActorKey, this.GetType());

            await OnActivate(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Deactivate actor
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        public async Task Deactivate(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            context = context.With(_tag);

            int currentValue = Interlocked.CompareExchange(ref _running, 0, 1);
            if (currentValue != 1)
            {
                return;
            }

            ActorManager.Configuration.ActorDeactivateEvent(context, ActorKey, this.GetType());
            StopTimer();
            await OnDeactivate(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose, virtual
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Event for on activate actor
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        protected virtual Task OnActivate(IWorkContext context) => Task.FromResult(0);

        /// <summary>
        /// Event for on deactivate actor
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        protected virtual Task OnDeactivate(IWorkContext context) => Task.FromResult(0);

        /// <summary>
        /// Time event
        /// </summary>
        /// <returns>task</returns>
        protected virtual Task OnTimer() => Task.FromResult(0);

        /// <summary>
        /// Set timer notification of actor
        /// </summary>
        /// <param name="dueTime">due time, first event</param>
        /// <param name="period">every period</param>
        public void SetTimer(TimeSpan dueTime, TimeSpan period)
        {
            _timer.Verify().Assert(x => x == null, "Timer already running");

            _timer = new Timer(TimerCallback, null, dueTime, period);
            ActorManager.Configuration.ActorStartTimerEvent(_workContext.WithMethodName(), ActorKey);
        }

        /// <summary>
        /// Stop timer
        /// </summary>
        public void StopTimer()
        {
            Timer? t = Interlocked.Exchange(ref _timer, null);
            if (t != null)
            {
                t.Dispose();
                ActorManager.Configuration.ActorStopTimerEvent(_workContext.WithMethodName(), ActorKey);
            }
        }

        /// <summary>
        /// Timer all back, through task
        /// </summary>
        /// <param name="obj">obj, not used</param>
        private void TimerCallback(object obj)
        {
            int currentValue = Interlocked.CompareExchange(ref _timerLockValue, 1, 0);
            if (currentValue != 0)
            {
                return;
            }

            try
            {
                ActorManager.Configuration.ActorStartTimerEvent(_workContext.WithMethodName(), ActorKey);
                OnTimer().GetAwaiter().GetResult();
            }
            finally
            {
                Interlocked.Exchange(ref _timerLockValue, 0);
            }
        }
    }
}
