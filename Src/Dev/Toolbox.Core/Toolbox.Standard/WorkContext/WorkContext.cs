// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            Cv = new CorrelationVector();
            Tag = StringVector.Empty;
            Telemetry = new TelemetryLogNull();
            Dimensions = EventDimensions.Empty;
        }

        /// <summary>
        /// Internal constructor for new instances
        /// </summary>
        /// <param name="workContext">copy from work context</param>
        private WorkContext(WorkContext workContext)
        {
            Cv = workContext.Cv;
            Tag = workContext.Tag;
            Container = workContext.Container;
            CancellationToken = workContext.CancellationToken;
            Telemetry = workContext.Telemetry;
            Dimensions = new EventDimensions(workContext.Dimensions);
        }

        /// <summary>
        /// Construct work context, for values that are not known to be immutable, shallow copies are made
        /// </summary>
        /// <param name="cv">correlation vector</param>
        /// <param name="tag">code location tag</param>
        /// <param name="container">container</param>
        /// <param name="properties">properties (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="eventLog"></param>
        /// <param name="dimensions"></param>
        public WorkContext(
            CorrelationVector cv,
            StringVector tag,
            IServiceProvider? container,
            CancellationToken? cancellationToken = null,
            ITelemetry? eventLog = null,
            IEventDimensions? dimensions = null
            )
        {
            cv.Verify().IsNotNull();
            cv.Verify().IsNotNull();

            Cv = cv;
            Tag = tag;
            Container = container;
            CancellationToken = cancellationToken ?? CancellationToken.None;
            Telemetry = eventLog ?? new TelemetryLogNull();
            Dimensions = dimensions != null ? new EventDimensions(dimensions) : new EventDimensions();
        }

        /// <summary>
        /// Static empty
        /// </summary>
        public static WorkContext Empty { get; } = new WorkContext();

        public CorrelationVector Cv { get; private set; }

        public StringVector Tag { get; private set; }

        public IServiceProvider? Container { get; }

        public CancellationToken CancellationToken { get; private set; } = CancellationToken.None;

        public ITelemetry Telemetry { get; private set; }

        public IEventDimensions Dimensions { get; private set; }

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
        /// Create a new context with a new CV
        /// </summary>
        /// <returns></returns>
        public IWorkContext WithNewCv()
        {
            return new WorkContext(this)
            {
                Cv = new CorrelationVector(),
            };
        }

        /// <summary>
        /// Create new instance of work context with Increment CV
        /// </summary>
        /// <returns>new work context</returns>
        public IWorkContext WithExtended()
        {
            return new WorkContext(this)
            {
                Cv = Cv.WithExtend(),
            };
        }

        /// <summary>
        /// Create new instance of work context with Increment CV
        /// </summary>
        /// <returns>new work context</returns>
        public IWorkContext WithIncrement()
        {
            return new WorkContext(this)
            {
                Cv = Cv.WithIncrement(),
            };
        }

        /// <summary>
        /// Create new instance of work context with method name being added to Tag
        /// </summary>
        /// <param name="memberName">member name (compiler will fill in)</param>
        /// <returns>new work context</returns>
        public IWorkContext WithMethodName([CallerMemberName] string? memberName = null)
        {
            return new WorkContext(this)
            {
                Tag = Tag.With(memberName!),
            };
        }

        /// <summary>
        /// Create new instance of work context with Tag being added to current Tag
        /// </summary>
        /// <param name="tag">code tag</param>
        /// <param name="memberName">method name (compiler will fill in)</param>
        /// <returns>new work context</returns>
        public IWorkContext WithTag(StringVector tag, [CallerMemberName] string? memberName = null)
        {
            tag.Verify(nameof(tag)).IsNotNull();

            return new WorkContext(this)
            {
                Tag = Tag.With(memberName!),
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
