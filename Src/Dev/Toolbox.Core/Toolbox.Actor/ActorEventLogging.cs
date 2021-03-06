﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace Khooversoft.Toolbox.Actor
{
    public static class ActorEventLogging
    {
        public static void ActorActivateEvent(this ActorConfiguration configuration, IWorkContext context, ActorKey actorKey, Type actorType, string? message = null)
        {
            configuration.Verify(nameof(configuration)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();
            actorType.Verify(nameof(actorType)).IsNotNull();

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Add(nameof(actorKey), actorKey)
                .Add(nameof(actorType), actorType)
                .Add(nameof(message), message)
                .Build();

            configuration.WorkContext.Telemetry.LogEvent(context, nameof(ActorActivateEvent), dimensions);
            configuration.WorkContext.Telemetry.TrackMetric(context, nameof(ActorActivateEvent), dimensions);
        }

        public static void ActorDeactivateEvent(this ActorConfiguration configuration, IWorkContext context, ActorKey actorKey, Type actorType, string? message = null)
        {
            configuration.Verify(nameof(configuration)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();
            actorType.Verify(nameof(actorType)).IsNotNull();

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Add(nameof(actorKey), actorKey)
                .Add(nameof(actorType), actorType)
                .Add(nameof(message), message)
                .Build();

            configuration.WorkContext.Telemetry.LogEvent(context, nameof(ActorDeactivateEvent), dimensions);
        }

        public static void ActorRegisteredEvent(this ActorConfiguration configuration, IWorkContext context, Type interfaceType, string? message = null)
        {
            configuration.Verify(nameof(configuration)).IsNotNull();
            interfaceType.Verify(nameof(interfaceType)).IsNotNull();

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Add(nameof(interfaceType), interfaceType)
                .Add(nameof(message), message)
                .Build();

            configuration.WorkContext.Telemetry.LogEvent(context, nameof(ActorRegisteredEvent), dimensions);
        }

        public static void ActorStartTimerEvent(this ActorConfiguration configuration, IWorkContext context, ActorKey actorKey, string? message = null)
        {
            configuration.Verify(nameof(configuration)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Add(nameof(actorKey), actorKey)
                .Add(nameof(message), message)
                .Build();

            configuration.WorkContext.Telemetry.LogEvent(context, nameof(ActorStartTimerEvent), dimensions);
        }

        public static void ActorStopTimerEvent(this ActorConfiguration configuration, IWorkContext context, ActorKey actorKey, string? message = null)
        {
            configuration.Verify(nameof(configuration)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();

            IEventDimensions dimensions = new EventDimensionsBuilder()
                .Add(nameof(actorKey), actorKey)
                .Add(nameof(message), message)
                .Build();

            configuration.WorkContext.Telemetry.LogEvent(context, nameof(ActorStopTimerEvent), dimensions);
        }
    }
}
