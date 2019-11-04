using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class TelemetryMessage
    {
        public TelemetryMessage(IWorkContext context, TelemetryType telemetryType, string eventSourceName, string eventName, string message, Exception exception, IEventDimensions eventDimensions)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            WorkContext = context;
            TelemetryType = telemetryType;
            EventSourceName = eventSourceName;
            EventName = eventName;
            Message = message;
            EventDimensions = (EventDimensions)eventDimensions + context.Dimensions;
            Exception = exception;
        }

        public TelemetryMessage(IWorkContext context, TelemetryType telemetryType, string eventSourceName, string eventName, string message, double value, IEventDimensions eventDimensions)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            WorkContext = context;
            TelemetryType = telemetryType;
            EventSourceName = eventSourceName;
            EventName = eventName;
            Message = message;
            Value = value;
            EventDimensions = (EventDimensions)eventDimensions + context.Dimensions;
        }

        public TelemetryMessage(TelemetryMessage message)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));

            MessageId = message.MessageId;
            EventDate = message.EventDate;
            WorkContext = message.WorkContext;
            TelemetryType = message.TelemetryType;
            EventSourceName = message.EventSourceName;
            EventName = message.EventName;
            Message = message.Message;
            Duration = message.Duration;
            Value = message.Value;
            EventDimensions = message.EventDimensions;
            Exception = message.Exception;
        }

        internal TelemetryMessage(MessageDeserialize messageDeserialize, IWorkContext context)
        {
            messageDeserialize = messageDeserialize ?? throw new ArgumentNullException(nameof(messageDeserialize));
            context = context ?? throw new ArgumentNullException(nameof(context));

            MessageId = messageDeserialize.MessageId;
            EventDate = messageDeserialize.EventDate;
            WorkContext = context;
            TelemetryType = messageDeserialize.TelemetryType;
            EventSourceName = messageDeserialize.EventSourceName;
            EventName = messageDeserialize.EventName;
            Message = messageDeserialize.Message;
            Duration = messageDeserialize.Duration;
            Value = messageDeserialize.Value;
            //EventDimensions = messageDeserialize.Dimensions?.ToEventDimensions();
        }

        public Guid MessageId { get; } = Guid.NewGuid();

        public DateTimeOffset EventDate { get; } = DateTimeOffset.UtcNow;

        public IWorkContext WorkContext { get; }

        public TelemetryType TelemetryType { get; private set; }

        public string? EventSourceName { get; }

        public string? EventName { get; }

        public string? Message { get; private set; }

        public long? Duration { get; }

        public double? Value { get; }

        public IEventDimensions? EventDimensions { get; }

        public Exception? Exception { get; }

        public IReadOnlyList<string> GetProperties()
        {
            var list = new List<string>
            {
                $"{nameof(MessageId)}={Message}",
                $"{nameof(EventDate)}={EventDate}",
                $"{nameof(TelemetryType)}={TelemetryType}",
                $"{nameof(EventSourceName)}={EventSourceName}",
                $"{nameof(WorkContext.Cv)}={WorkContext.Cv}",
                $"{nameof(WorkContext.Tag)}={WorkContext.Tag}",
                $"{nameof(EventName)}={EventName}",
                $"{nameof(Duration)}={Duration?.ToString() ?? "<none>"}",
                $"{nameof(Value)}={Value?.ToString() ?? "<none>"}",
                $"{nameof(MessageId)}={Message ?? "<none>"}",
                $"{nameof(Exception)}={Exception?.ToString() ?? "<none>"}",
            };

            EventDimensions?.ForEach(x => list.Add($"{x.Key}={x.Value}"));

            return list;
        }

        public override string ToString()
        {
            return string.Join(", ", GetProperties());
        }

        public TelemetryMessage WithReplay()
        {
            return new TelemetryMessage(this)
            {
                TelemetryType = TelemetryType.Replay | TelemetryType
            };
        }
    }
}
