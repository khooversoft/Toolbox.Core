// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Actor
{
    public class ActorConfiguration
    {
        public ActorConfiguration(
            int capacity,
            IActorRepository actorRepository,
            TimeSpan actorCallTimeout,
            TimeSpan actorRetirementPeriod,
            TimeSpan inactivityScanPeriod,
            IWorkContext workContext
            )
        {
            workContext.Verify(nameof(workContext)).IsNotNull();

            Capacity = capacity;
            ActorRepository = actorRepository;
            ActorCallTimeout = actorCallTimeout;
            ActorRetirementPeriod = actorRetirementPeriod;
            InactivityScanPeriod = inactivityScanPeriod;
            WorkContext = workContext;
        }

        public int Capacity { get; }

        public IActorRepository ActorRepository { get; }

        public TimeSpan ActorCallTimeout { get; }

        public TimeSpan ActorRetirementPeriod { get; }

        public TimeSpan InactivityScanPeriod { get; }

        public IWorkContext WorkContext { get; }
    }
}
