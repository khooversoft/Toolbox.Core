// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Actor
{
    public static class ActorManagerExtensions
    {
        public static IActorManager ToActorManager(this ActorConfiguration self)
        {
            self.Verify(nameof(self)).IsNotNull();

            return new ActorManager(self);
        }
    }
}
