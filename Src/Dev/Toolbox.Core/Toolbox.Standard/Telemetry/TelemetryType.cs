// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Identifies the level of an event.
    /// </summary>
    [Flags]
    public enum TelemetryType
    {
        None = 0,

        /// <summary>
        /// Critical error
        /// </summary>
        Critical = 0x1,

        /// <summary>
        /// Error, maybe recoverable
        /// </summary>
        Error = 0x2,

        /// <summary>
        /// Warning message
        /// </summary>
        Warning = 0x3,

        /// <summary>
        /// General purpose information
        /// </summary>
        Informational = 0x4,

        /// <summary>
        /// Verbose level, this level adds lengthy events or messages. It causes all events to be logged.
        /// </summary>
        Verbose = 0x5,

        /// <summary>
        /// Metric type
        /// </summary>
        Metric = 0x6,

        /// <summary>
        /// Event type
        /// </summary>
        Event = 0x1000,

        /// <summary>
        /// Replay log item
        /// </summary>
        Replay = 0x2000,

        LevelMask = 0x7,

        CriticalEvent = Critical | Event,
        ErrorEvent = Error | Event,
        WarningEvent = Warning | Event,
        InformationEvent = Informational | Event,
    }

    public static class TelemetryFilters
    {
        public static bool FilterLevel(this TelemetryType telemetryType, TelemetryType level) => (telemetryType & TelemetryType.LevelMask) <= level;

        public static bool IsVerbose(this TelemetryType telemetryType) => (telemetryType & TelemetryType.LevelMask) == TelemetryType.Verbose;
        public static bool IsReplay(this TelemetryType telemetryType) => (telemetryType & TelemetryType.Replay) == TelemetryType.Replay;
        public static bool IsEvent(this TelemetryType telemetryType) => (telemetryType & TelemetryType.Event) == TelemetryType.Event;
        public static bool IsErrorOrCritical(this TelemetryType telemetryType) => FilterLevel(telemetryType, TelemetryType.Error);
        public static bool IsMetric(this TelemetryType telemetryType) => (telemetryType & TelemetryType.LevelMask) == TelemetryType.Metric;
    }
}
