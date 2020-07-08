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
            TimeSpan actorCallTimeout,
            TimeSpan actorRetirementPeriod,
            TimeSpan inactivityScanPeriod
            )
        {
            Capacity = capacity;
            ActorCallTimeout = actorCallTimeout;
            ActorRetirementPeriod = actorRetirementPeriod;
            InactivityScanPeriod = inactivityScanPeriod;
        }

        public int Capacity { get; }

        public TimeSpan ActorCallTimeout { get; }

        public TimeSpan ActorRetirementPeriod { get; }

        public TimeSpan InactivityScanPeriod { get; }
    }
}
