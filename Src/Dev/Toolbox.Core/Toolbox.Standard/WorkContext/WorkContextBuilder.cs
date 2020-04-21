// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

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
        static WorkContextBuilder()
        {
            var builder = new WorkContextBuilder();
            builder.ActivityId = nameof(WorkContextBuilder).ToGuid();
            builder.ParentActivityId = builder.ActivityId;

            Default = builder.Build();
        }

        /// <summary>
        /// Default construct
        /// </summary>
        public WorkContextBuilder()
        {
            ActivityId = Guid.NewGuid();
            ParentActivityId = ActivityId;
            Telemetry = new TelemetryLogNull();
            Dimensions = new EventDimensions();
        }

        /// <summary>
        /// Construct from work context
        /// </summary>
        /// <param name="context"></param>
        public WorkContextBuilder(IWorkContext context)
        {
            context.VerifyNotNull(nameof(context));

            ActivityId = context.ActivityId;
            ParentActivityId = context.ParentActivityId;
            Container = context.Container;
            CancellationToken = context.CancellationToken;
            Telemetry = context.Telemetry;
            Dimensions = context.Dimensions;
        }

        public static IWorkContext Default { get; }

        public Guid ActivityId { get; set; }

        public Guid ParentActivityId { get; set; }

        public IServiceContainer? Container { get; set; }

        public CancellationToken? CancellationToken { get; set; }

        public ITelemetry Telemetry { get; set; }

        public IEventDimensions Dimensions { get; set; }

        /// <summary>
        /// Set event log
        /// </summary>
        /// <param name="telemetry">event log</param>
        /// <returns>this</returns>
        public WorkContextBuilder Set(ITelemetry telemetry)
        {
            telemetry.VerifyNotNull(nameof(telemetry));

            Telemetry = telemetry;
            return this;
        }

        /// <summary>
        /// Set event dimensions
        /// </summary>
        /// <param name="values">dimensions values</param>
        /// <returns>this</returns>
        public WorkContextBuilder Set(IEventDimensions eventDimension)
        {
            eventDimension.VerifyNotNull(nameof(eventDimension));

            Dimensions = new EventDimensions(eventDimension);
            return this;
        }

        /// <summary>
        /// Set container (AutoFac)
        /// </summary>
        /// <param name="container">container</param>
        /// <returns>this</returns>
        public WorkContextBuilder Set(IServiceContainer container)
        {
            container.VerifyNotNull(nameof(container));

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
                activityId: ActivityId,
                parentActivityId: ParentActivityId,
                container: Container,
                CancellationToken,
                telemetry: Telemetry,
                dimensions: new EventDimensions(Dimensions)
                );
        }
    }
}
