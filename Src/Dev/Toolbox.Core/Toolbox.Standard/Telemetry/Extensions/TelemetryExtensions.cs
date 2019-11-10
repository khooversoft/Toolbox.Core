using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class TelemetryExtensions
    {
        public static IWorkContext WithCreateLogger(this IWorkContext context, string eventSourceName)
        {
            context.Verify(nameof(context)).IsNotNull();
            eventSourceName.Verify(nameof(eventSourceName)).IsNotEmpty();

            if (context.Container == null) return context;

            ITelemetry telementry = context.Container.Resolve<ITelemetryService>()
                 .CreateLogger(eventSourceName);

            return context.With(telementry);
        }

        public static IEventDimensions ToEventDimensions(this IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            return new EventDimensions(keyValuePairs);
        }

        public static IEventDimensions ToEventDimensions(this object dimensions)
        {
            dimensions.Verify(nameof(dimensions)).IsNotNull();

            return dimensions.ToKeyValues().ToEventDimensions();
        }
    }
}
