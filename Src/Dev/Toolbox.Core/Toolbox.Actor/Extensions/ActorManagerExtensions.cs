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
