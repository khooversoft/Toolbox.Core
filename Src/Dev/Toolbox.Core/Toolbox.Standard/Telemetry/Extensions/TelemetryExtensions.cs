//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Khooversoft.Toolbox.Standard
//{
//    public static class TelemetryExtensions
//    {
//        public static IWorkContext WithCreateLogger(this IWorkContext context, string eventSourceName)
//        {
//            context.Verify(nameof(context)).IsNotNull();
//            eventSourceName.Verify(nameof(eventSourceName)).IsNotEmpty();

//            if (context.Container == null)
//            {
//                return context;
//            }

//            ITelemetry telementry = context.Container.Resolve<ITelemetryService>()
//                 .CreateLogger(eventSourceName);

//            return context.With(telementry);
//        }

//        public static IEventDimensions ToEventDimensions(this IEnumerable<KeyValuePair<string, object>> keyValuePairs)
//        {
//            return new EventDimensions(keyValuePairs);
//        }

//        public static IEventDimensions ToEventDimensions(this object dimensions)
//        {
//            Verify.IsNotNull(nameof(dimensions), dimensions);

//            return dimensions.ToKeyValues().ToEventDimensions();
//        }

//        public static IReadOnlyDictionary<string, string> GetTelemetryProperties(this TelemetryMessage telemetryMessage)
//        {
//            var list = new Dictionary<string, string>
//            {
//                [nameof(TelemetryMessage.Message)] = telemetryMessage.Message,
//                [nameof(TelemetryMessage.MessageId)] = telemetryMessage.MessageId.ToString(),
//                [nameof(TelemetryMessage.EventDate)] = telemetryMessage.EventDate.ToString(),
//                [nameof(TelemetryMessage.TelemetryType)] = telemetryMessage.TelemetryType.ToString(),
//                [nameof(TelemetryMessage.EventSourceName)] = telemetryMessage.EventSourceName,
//                [nameof(TelemetryMessage.WorkContext.Cv)] = telemetryMessage.WorkContext.Cv,
//                [nameof(TelemetryMessage.WorkContext.Tag)] = telemetryMessage.WorkContext.Tag,
//                [nameof(TelemetryMessage.EventName)] = telemetryMessage.EventName,
//                [nameof(TelemetryMessage.Duration)] = telemetryMessage.Duration?.ToString() ?? "<none>",
//                [nameof(TelemetryMessage.Value)] = telemetryMessage.Value?.ToString() ?? "<none>",
//                [nameof(TelemetryMessage.Exception)] = telemetryMessage.Exception?.ToString() ?? "<none>",
//            };

//            telemetryMessage.EventDimensions?.ForEach(x => list.Add(x.Key, x.Value?.ToString() ?? "<none>"));

//            return list;
//        }
//    }
//}
