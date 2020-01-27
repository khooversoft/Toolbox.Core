// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public static class ActorManagerExtensions
    {
        public static IActorManager ToActorManager(this ActorConfiguration self)
        {
            self.Verify(nameof(self)).IsNotNull();

            return new ActorManager(self);
        }

        /// <summary>
        /// Create proxy to actor, return current instance or create one
        /// </summary>
        /// <typeparam name="T">actor interface </typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns></returns>
        public static Task<T> GetActor<T>(this IActorManager actorManager, string actorKey) where T : IActor
        {
            return actorManager.GetActor<T>(new ActorKey(actorKey));
        }

        /// <summary>
        /// Does actor instance exist?
        /// </summary>
        /// <typeparam name="T">interface of actor</typeparam>
        /// <param name="actorKey">actor key</param>
        /// <returns></returns>
        public static bool Exist<T>(this IActorManager actorManager, string actorKey) where T : IActor
        {
            return actorManager.Exist<T>(new ActorKey(actorKey));
        }
    }
}
