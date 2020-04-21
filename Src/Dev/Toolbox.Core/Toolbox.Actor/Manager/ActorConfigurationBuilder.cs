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
            ActorRepository = configuration.ActorRepository;
            ActorCallTimeout = configuration.ActorCallTimeout;
            ActorRetirementPeriod = configuration.ActorRetirementPeriod;
            InactivityScanPeriod = configuration.InactivityScanPeriod;
            WorkContext = configuration.WorkContext;
        }

        public static ActorConfiguration Default { get; } = new ActorConfigurationBuilder().Build();

        /// <summary>
        /// Maximum number of actors the manager can handle
        /// </summary>
        public int Capacity { get; private set; } = 10000;

        /// <summary>
        /// Repository for the actor types and instances
        /// </summary>
        public IActorRepository? ActorRepository { get; private set; }

        /// <summary>
        /// Timeout for actor calls
        /// </summary>
        public TimeSpan ActorCallTimeout { get; private set; } = TimeSpan.FromSeconds(120);

        /// <summary>
        /// Time span that inactive actors can be retried.  Each time an actor is accessed,
        /// its time stamp is updated.  Actors are collected with this time span is exceeded.
        /// </summary>
        public TimeSpan ActorRetirementPeriod { get; private set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// Schedule for when inactive actors are destroyed
        /// </summary>
        public TimeSpan InactivityScanPeriod { get; private set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Work context for manager
        /// </summary>
        public IWorkContext? WorkContext { get; private set; } = WorkContextBuilder.Default;

        public ActorConfigurationBuilder Set(int capacity)
        {
            Capacity = capacity;
            return this;
        }

        public ActorConfigurationBuilder Set(IActorRepository repository)
        {
            ActorRepository = repository;
            return this;
        }

        public ActorConfigurationBuilder Set(IWorkContext context)
        {
            WorkContext = context;
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
                actorRepository: ActorRepository!,
                actorCallTimeout: ActorCallTimeout,
                actorRetirementPeriod: ActorRetirementPeriod,
                inactivityScanPeriod: InactivityScanPeriod,
                workContext: WorkContext!
            );
        }
    }
}
