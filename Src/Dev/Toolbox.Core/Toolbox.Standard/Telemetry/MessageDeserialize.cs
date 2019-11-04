using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    internal class MessageDeserialize
    {
        public Guid MessageId { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public string? Cv { get; set; }

        public string? Tag { get; set; }

        public TelemetryType TelemetryType { get; set; }

        public string? EventSourceName { get; set; }

        public string? EventName { get; set; }

        public string? Message { get; set; }

        public long? Duration { get; set; }

        public double? Value { get; set; }

        public IReadOnlyDictionary<string, object>? Dimensions { get; set; }
    }
}
