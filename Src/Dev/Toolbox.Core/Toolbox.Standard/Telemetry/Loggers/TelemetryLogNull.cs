// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class TelemetryLogNull : ITelemetry
    {
        public TelemetryLogNull()
        {
        }

        public string EventSourceName => "null";

        public void ActivityStart(IWorkContext context, string? message = null, IEventDimensions? dimensions = null)
        {
        }

        public void ActivityStop(IWorkContext context, string? message = null, long durationMs = 0, IEventDimensions? dimensions = null)
        {
        }

        public void Critical(IWorkContext context, string message, Exception? exception = null, IEventDimensions? dimensions = null)
        {
        }

        public void Error(IWorkContext context, string message, Exception? exception = null, IEventDimensions? dimensions = null)
        {
        }

        public void Info(IWorkContext context, string message, IEventDimensions? dimensions = null)
        {
        }

        public void LogCritialEvent(IWorkContext context, string eventName, IEventDimensions? dimensions = null, string? message = null)
        {
        }

        public void LogErrorEvent(IWorkContext context, string eventName, IEventDimensions? dimensions = null, string? message = null)
        {
        }

        public void LogEvent(IWorkContext context, string eventName, IEventDimensions? dimensions = null, string? message = null)
        {
        }

        public void TrackMetric(IWorkContext context, string name, IEventDimensions? dimensions = null, string? message = null)
        {
        }

        public void TrackMetric(IWorkContext context, string name, double value, IEventDimensions? dimensions = null, string? message = null)
        {
        }

        public void Verbose(IWorkContext context, string message, IEventDimensions? dimensions = null)
        {
        }

        public void Warning(IWorkContext context, string message, IEventDimensions? dimensions = null)
        {
        }
    }
}
