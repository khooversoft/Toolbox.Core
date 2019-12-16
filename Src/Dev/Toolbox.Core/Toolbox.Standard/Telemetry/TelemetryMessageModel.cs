// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    internal class TelemetryMessageModel
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

        public Exception? Exception { get; set; }
    }
}
