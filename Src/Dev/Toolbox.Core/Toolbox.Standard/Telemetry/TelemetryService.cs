using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public class TelemetryService : ITelemetryService
    {
        private readonly IPipelineBlock<TelemetryMessage> _pipeline;
        private Func<TelemetryMessage, TelemetryMessage> _transform = x => x;

        public TelemetryService()
        {
            _pipeline = new PipelineBlock<TelemetryMessage>()
                .Select(_transform);
        }

        public ITelemetry CreateLogger(string eventSourceName)
        {
            eventSourceName.Verify(nameof(eventSourceName)).IsNotEmpty();

            return new LoggerProxy(this, eventSourceName);
        }

        public void Write(TelemetryMessage message)
        {
            message.Verify(nameof(message)).IsNotNull();

            _pipeline.Post(message);
        }

        public TelemetryService SetTransform(Func<TelemetryMessage, TelemetryMessage> transform)
        {
            transform.Verify(nameof(transform)).IsNotNull();

            _transform = transform;
            return this;
        }

        public TelemetryService AddConsoleLogger(TelemetryType telemetryType, ITelemetryLogger logger)
        {
            logger.Verify(nameof(logger)).IsNotNull();

            _pipeline.DoAction(x => logger.Write(x), x => x.TelemetryType.IsReplay() || x.TelemetryType.FilterLevel(telemetryType));

            return this;
        }

        public TelemetryService AddFileLogger(ITelemetryLogger logger)
        {
            logger.Verify(nameof(logger)).IsNotNull();

            _pipeline.DoAction(x => logger.Write(x), x => !x.TelemetryType.IsReplay() && !x.TelemetryType.IsMetric());

            return this;
        }

        public TelemetryService AddEventLogger(ITelemetryLogger logger)
        {
            logger.Verify(nameof(logger)).IsNotNull();

            _pipeline.DoAction(x => logger.Write(x), x => !x.TelemetryType.IsReplay() && x.TelemetryType.IsEvent() && !x.TelemetryType.IsVerbose());

            return this;
        }

        public TelemetryService AddErrorReplay(IWorkContext context, TelemetryType console, ITelemetryQuery logger, ITelemetryService telemetryService)
        {
            context.Verify(nameof(context)).IsNotNull();
            logger.Verify(nameof(logger)).IsNotNull();
            telemetryService.Verify(nameof(telemetryService)).IsNotNull();

            void action(TelemetryMessage x)
            {
                IReadOnlyList<TelemetryMessage> loggedMessages = logger.Query(y => x.WorkContext.Cv == y.WorkContext.Cv, 30, 100);

                loggedMessages.ForEach(y => telemetryService.Write(y.WithReplay()));
            }

            bool filter(TelemetryMessage x) =>
                    (console != TelemetryType.Verbose) &&
                    (!x.TelemetryType.IsReplay()) &&
                    (x.TelemetryType.IsEvent()) &&
                    (x.TelemetryType.IsErrorOrCritical()) &&
                    (x?.WorkContext?.Cv != null);

            _pipeline.DoAction(action, filter);

            return this;
        }
    }
}
