// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public interface ITelemetry
    {
        string EventSourceName { get; }

        void ActivityStart(IWorkContext context, string? message = null, IEventDimensions? dimensions = null);

        void ActivityStop(IWorkContext context, string? message = null, long durationMs = 0, IEventDimensions? dimensions = null);

        void Verbose(IWorkContext context, string message, IEventDimensions? dimensions = null);

        void Info(IWorkContext context, string message, IEventDimensions? dimensions = null);

        void Warning(IWorkContext context, string message, IEventDimensions? dimensions = null);

        void Error(IWorkContext context, string message, Exception? exception = null, IEventDimensions? dimensions = null);

        void Critical(IWorkContext context, string message, Exception? exception = null, IEventDimensions? dimensions = null);

        void TrackMetric(IWorkContext context, string name, IEventDimensions? dimensions = null, string? message = null);

        void TrackMetric(IWorkContext context, string name, double value, IEventDimensions? dimensions = null, string? message = null);

        void LogEvent(IWorkContext context, string eventName, IEventDimensions? dimensions = null, string? message = null);

        void LogErrorEvent(IWorkContext context, string eventName, IEventDimensions? dimensions = null, string? message = null);

        void LogCritialEvent(IWorkContext context, string eventName, IEventDimensions? dimensions = null, string? message = null);
    }
}
