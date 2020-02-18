// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Immutable execution context
    /// </summary>
    public sealed class WorkContext : IWorkContext
    {
        /// <summary>
        /// Default constructor for Empty
        /// </summary>
        public WorkContext()
        {
            ActivityId = Guid.NewGuid();
            ParentActivityId = ActivityId;

            Telemetry = new TelemetryLogNull();
            Dimensions = EventDimensions.Empty;
        }

        /// <summary>
        /// Internal constructor for new instances
        /// </summary>
        /// <param name="workContext">copy from work context</param>
        private WorkContext(WorkContext workContext)
        {
            ActivityId = workContext.ActivityId;
            ParentActivityId = workContext.ParentActivityId;

            Container = workContext.Container;
            CancellationToken = workContext.CancellationToken;
            Telemetry = workContext.Telemetry;
            Dimensions = new EventDimensions(workContext.Dimensions);
        }

        /// <summary>
        /// Construct work context, for values that are not known to be immutable, shallow copies are made
        /// </summary>
        /// <param name="activityId">correlation vector</param>
        /// <param name="parentActivityId">code location tag</param>
        /// <param name="container">container</param>
        /// <param name="properties">properties (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="telemetry"></param>
        /// <param name="dimensions"></param>
        public WorkContext(
            Guid activityId,
            Guid parentActivityId,

            IServiceContainer? container,
            CancellationToken? cancellationToken = null,
            ITelemetry? telemetry = null,
            IEventDimensions? dimensions = null
            )
        {
            ActivityId = activityId;
            ParentActivityId = parentActivityId;

            Container = container;
            CancellationToken = cancellationToken ?? CancellationToken.None;
            Telemetry = telemetry ?? new TelemetryLogNull();
            Dimensions = dimensions != null ? new EventDimensions(dimensions) : new EventDimensions();
        }

        public Guid ActivityId { get; private set; }

        public Guid ParentActivityId { get; private set; }

        public IServiceContainer? Container { get; }

        public CancellationToken CancellationToken { get; private set; } = CancellationToken.None;

        public ITelemetry Telemetry { get; private set; }

        public IEventDimensions Dimensions { get; private set; }

        /// <summary>
        /// With new activity id, switches with parent activity id
        /// </summary>
        /// <returns></returns>
        public IWorkContext WithActivity()
        {
            var context = new WorkContext(this);
            context.ParentActivityId = ActivityId;
            context.ActivityId = Guid.NewGuid();

            return context;
        }

        /// <summary>
        /// Create new context with cancellation token
        /// </summary>
        /// <param name="token">token to set, null to clear</param>
        /// <returns>new work context</returns>
        public IWorkContext With(CancellationToken? token)
        {
            return new WorkContext(this)
            {
                CancellationToken = token ?? CancellationToken.None
            };
        }

        /// <summary>
        /// Create new context with cancellation token
        /// </summary>
        /// <param name="eventLog">event log to use</param>
        /// <returns>new work context</returns>
        public IWorkContext With(ITelemetry eventLog)
        {
            eventLog.Verify(nameof(eventLog)).IsNotNull();

            return new WorkContext(this)
            {
                Telemetry = eventLog,
            };
        }

        /// <summary>
        /// Create new context with cancellation token
        /// </summary>
        /// <param name="eventDimenensions">event log to use</param>
        /// <returns>new work context</returns>
        public IWorkContext With(IEventDimensions eventDimenensions)
        {
            eventDimenensions.Verify(nameof(eventDimenensions)).IsNotNull();

            return new WorkContext(this)
            {
                Dimensions = new EventDimensions(Dimensions) + eventDimenensions,
            };
        }

        /// <summary>
        /// Create work context builder class from current work context
        /// </summary>
        /// <returns>builder class</returns>
        public WorkContextBuilder ToBuilder()
        {
            return new WorkContextBuilder(this);
        }
    }
}
