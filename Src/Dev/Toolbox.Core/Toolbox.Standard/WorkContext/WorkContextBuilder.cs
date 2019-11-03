// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Work context builder, used this class to create new instances of immutable work context
    /// </summary>
    public class WorkContextBuilder
    {
        /// <summary>
        /// Default construct
        /// </summary>
        public WorkContextBuilder()
        {
            Cv = new CorrelationVector();
            Tag = StringVector.Empty;
            EventLog = new TelemetryLogNull();
            Dimensions = new EventDimensions();
        }

        /// <summary>
        /// Construct from work context
        /// </summary>
        /// <param name="context"></param>
        public WorkContextBuilder(IWorkContext context)
        {
            context.Verify().IsNotNull();

            Cv = context.Cv;
            Tag = context.Tag;
            Container = context.Container;
            CancellationToken = context.CancellationToken;
            EventLog = context.Telemetry;
            Dimensions = context.Dimensions;
        }

        public CorrelationVector Cv { get; set; }

        public StringVector Tag { get; set; }

        public IServiceProvider? Container { get; set; }

        public CancellationToken? CancellationToken { get; set; }

        public ITelemetry EventLog { get; set; }

        public IEventDimensions Dimensions { get; set; }

        /// <summary>
        /// Set code tag
        /// </summary>
        /// <param name="tag">code tag</param>
        /// <returns>this</returns>
        public WorkContextBuilder Set(StringVector tag)
        {
            tag.Verify().IsNotNull();

            Tag = Tag.With(tag);
            return this;
        }

        /// <summary>
        /// Set correlation vector
        /// </summary>
        /// <param name="cv">cv</param>
        /// <returns>this</returns>
        public WorkContextBuilder Set(CorrelationVector cv)
        {
            cv.Verify().IsNotNull();

            Cv = cv;
            return this;
        }

        /// <summary>
        /// Set event log
        /// </summary>
        /// <param name="eventLog">event log</param>
        /// <returns>this</returns>
        public WorkContextBuilder Set(ITelemetry eventLog)
        {
            eventLog.Verify().IsNotNull();

            EventLog = eventLog;
            return this;
        }

        /// <summary>
        /// Set event dimensions
        /// </summary>
        /// <param name="values">dimensions values</param>
        /// <returns>this</returns>
        public WorkContextBuilder Set(IEventDimensions eventDimension)
        {
            eventDimension.Verify().IsNotNull();

            Dimensions = new EventDimensions(eventDimension);
            return this;
        }

        /// <summary>
        /// Set container (AutoFac)
        /// </summary>
        /// <param name="container">container</param>
        /// <returns>this</returns>
        public WorkContextBuilder Set(IServiceProvider container)
        {
            container.Verify().IsNotNull();

            Container = container;
            return this;
        }

        /// <summary>
        /// Set cancellation token
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns></returns>
        public WorkContextBuilder Set(CancellationToken? token)
        {
            CancellationToken = token;
            return this;
        }

        /// <summary>
        /// Build immutable work context from details
        /// </summary>
        /// <returns>new instance of work context</returns>
        public IWorkContext Build()
        {
            return new WorkContext(
                cv: Cv,
                tag: Tag,
                container: Container,
                CancellationToken,
                eventLog: EventLog,
                dimensions: new EventDimensions(Dimensions)
                );
        }
    }
}
