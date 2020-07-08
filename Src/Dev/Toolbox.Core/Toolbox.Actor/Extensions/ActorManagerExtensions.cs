// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public static class ActorManagerExtensions
    {
        /// <summary>
        /// Create proxy to actor, return current instance or create one
        /// </summary>
        /// <typeparam name="T">actor interface </typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns></returns>
        public static T GetActor<T>(this IActorManager actorManager, string actorKey) where T : IActor
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

        public static IActorManager ToActorManager(this ActorConfiguration self, ILoggerFactory loggerFactory)
        {
            self.VerifyNotNull(nameof(self));

            return new ActorManager(self, loggerFactory);
        }
    }
}
