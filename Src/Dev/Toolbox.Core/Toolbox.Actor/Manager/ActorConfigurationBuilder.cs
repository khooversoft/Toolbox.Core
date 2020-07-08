// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Builds the configuration for an actor manager
    /// </summary>
    public class ActorConfigurationBuilder
    {
        public ActorConfigurationBuilder()
        {
        }

        public ActorConfigurationBuilder(ActorConfiguration configuration)
        {
            configuration.VerifyNotNull(nameof(configuration));

            Capacity = configuration.Capacity;
            ActorCallTimeout = configuration.ActorCallTimeout;
            ActorRetirementPeriod = configuration.ActorRetirementPeriod;
            InactivityScanPeriod = configuration.InactivityScanPeriod;
        }

        public static ActorConfiguration Default { get; } = new ActorConfigurationBuilder().Build();

        /// <summary>
        /// Maximum number of actors the manager can handle
        /// </summary>
        public int Capacity { get; set; } = 10000;

        /// <summary>
        /// Timeout for actor calls
        /// </summary>
        public TimeSpan ActorCallTimeout { get; set; } = TimeSpan.FromSeconds(120);

        /// <summary>
        /// Time span that inactive actors can be retried.  Each time an actor is accessed,
        /// its time stamp is updated.  Actors are collected with this time span is exceeded.
        /// </summary>
        public TimeSpan ActorRetirementPeriod { get; set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// Schedule for when inactive actors are destroyed
        /// </summary>
        public TimeSpan InactivityScanPeriod { get; set; } = TimeSpan.FromMinutes(5);

        public ActorConfigurationBuilder Set(int capacity)
        {
            Capacity = capacity;
            return this;
        }

        public ActorConfigurationBuilder SetActorCallTimeout(TimeSpan span)
        {
            ActorCallTimeout = span;
            return this;
        }

        public ActorConfigurationBuilder SetActorRetirementPeriod(TimeSpan span)
        {
            ActorRetirementPeriod = span;
            return this;
        }

        public ActorConfigurationBuilder SetInactivityScanPeriod(TimeSpan span)
        {
            InactivityScanPeriod = span;
            return this;
        }

        public ActorConfiguration Build()
        {
            return new ActorConfiguration(
                capacity: Capacity,
                actorCallTimeout: ActorCallTimeout,
                actorRetirementPeriod: ActorRetirementPeriod,
                inactivityScanPeriod: InactivityScanPeriod
            );
        }
    }
}
