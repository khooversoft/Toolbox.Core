// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Khooversoft.Toolbox.Standard
//{
//    /// <summary>
//    /// Internal proxy logger, used by event source to send telemetry
//    /// </summary>
//    internal class LoggerProxy : ITelemetry
//    {
//        private readonly IPipeline<IWorkContext, TelemetryMessage> _manager;

//        public LoggerProxy(IPipeline<IWorkContext, TelemetryMessage> manager, string eventSourceName)
//        {
//            Verify.IsNotNull(nameof(manager), manager);

//            _manager = manager;
//            EventSourceName = eventSourceName;
//        }

//        public string EventSourceName { get; }

//        public void ActivityStart(IWorkContext context, string message = null, IEventDimensions dimensions = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.Metric, EventSourceName, nameof(ActivityStart), message, dimensions));
//        }

//        public void ActivityStop(IWorkContext context, string message = null, long durationMs = 0, IEventDimensions dimensions = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.Metric, EventSourceName, nameof(ActivityStop), message, durationMs, dimensions));
//        }

//        public void Critical(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.Critical, EventSourceName, nameof(Critical), message, exception, dimensions));
//        }

//        public void Error(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.Error, EventSourceName, nameof(Error), message, exception, dimensions));
//        }

//        public void Info(IWorkContext context, string message, IEventDimensions dimensions = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.Informational, EventSourceName, nameof(Info), message, dimensions));
//        }

//        public void Verbose(IWorkContext context, string message, IEventDimensions dimensions = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.Verbose, EventSourceName, nameof(Verbose), message, dimensions));
//        }

//        public void Warning(IWorkContext context, string message, IEventDimensions dimensions = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.Warning, EventSourceName, nameof(Warning), message, dimensions));
//        }

//        public void TrackMetric(IWorkContext context, string name, IEventDimensions dimensions = null, string message = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.Metric, EventSourceName, name, message, dimensions));
//        }

//        public void TrackMetric(IWorkContext context, string name, double value, IEventDimensions dimensions = null, string message = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.Metric, EventSourceName, name, message, value, dimensions?.ToEventDimensions()));
//        }

//        public void LogEvent(IWorkContext context, string eventName, IEventDimensions dimensions = null, string message = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.InformationEvent, EventSourceName, eventName, message, dimensions));
//        }

//        public void LogErrorEvent(IWorkContext context, string eventName, IEventDimensions dimensions = null, string message = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.ErrorEvent, EventSourceName, eventName, message, dimensions));
//        }

//        public void LogCritialEvent(IWorkContext context, string eventName, IEventDimensions dimensions = null, string message = null)
//        {
//            _manager.Post(context, new TelemetryMessage(context, TelemetryType.CriticalEvent, EventSourceName, eventName, message, dimensions));
//        }
//    }
//}
