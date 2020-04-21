﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class TelemetryServiceBuilder
    {
        private readonly DataflowBuilder<TelemetryMessage> _dataflowBuilder;

        public TelemetryServiceBuilder()
        {
            _dataflowBuilder = new DataflowBuilder<TelemetryMessage>();
        }

        public TelemetryServiceBuilder(Func<TelemetryMessage, TelemetryMessage> transform)
        {
            transform.VerifyNotNull(nameof(transform));

            _dataflowBuilder = new DataflowBuilder<TelemetryMessage>
            {
                new SelectDataflow<TelemetryMessage>(transform),
            };
        }

        public TelemetryServiceBuilder DoAction(Action<TelemetryMessage> action, Func<TelemetryMessage, bool>? predicate = null)
        {
            if (predicate == null) predicate = x => true;

            _dataflowBuilder.Add(new ActionDataflow<TelemetryMessage>(action, predicate));
            return this;
        }

        public TelemetryServiceBuilder AddConsoleLogger(TelemetryType telemetryType, ITelemetryLogger logger)
        {
            logger.VerifyNotNull(nameof(logger));

            _dataflowBuilder.Add(new ActionDataflow<TelemetryMessage>(x => logger.Write(x), x => x.TelemetryType.IsReplay() || x.TelemetryType.FilterLevel(telemetryType)));
            return this;
        }

        public TelemetryServiceBuilder AddFileLogger(ITelemetryLogger logger)
        {
            logger.VerifyNotNull(nameof(logger));

            _dataflowBuilder.Add(new ActionDataflow<TelemetryMessage>(x => logger.Write(x), x => !x.TelemetryType.IsReplay() && !x.TelemetryType.IsMetric()));
            return this;
        }

        public TelemetryServiceBuilder AddEventLogger(ITelemetryLogger logger)
        {
            logger.VerifyNotNull(nameof(logger));

            _dataflowBuilder.Add(new ActionDataflow<TelemetryMessage>(x => logger.Write(x), x => !x.TelemetryType.IsReplay() && x.TelemetryType.IsEvent() && !x.TelemetryType.IsVerbose()));
            return this;
        }

        public TelemetryServiceBuilder AddErrorReplay(IWorkContext context, TelemetryType console, ITelemetryQuery logger, ITelemetryService telemetryService)
        {
            context.VerifyNotNull(nameof(context));
            logger.VerifyNotNull(nameof(logger));
            telemetryService.VerifyNotNull(nameof(telemetryService));

            void action(TelemetryMessage x)
            {
                IReadOnlyList<TelemetryMessage> loggedMessages = logger.Query(y => x.WorkContext.ActivityId == y.WorkContext.ActivityId, 30, 100);

                loggedMessages.ForEach(y => telemetryService.Write(y.WithReplay()));
            }

            bool filter(TelemetryMessage x) =>
                    (console != TelemetryType.Verbose) &&
                    (!x.TelemetryType.IsReplay()) &&
                    (x.TelemetryType.IsEvent()) &&
                    (x.TelemetryType.IsErrorOrCritical());

            _dataflowBuilder.Add(new ActionDataflow<TelemetryMessage>(action, filter));
            return this;
        }

        public ITelemetryService Build()
        {
            return new TelemetryService(_dataflowBuilder.Build());
        }
    }
}
