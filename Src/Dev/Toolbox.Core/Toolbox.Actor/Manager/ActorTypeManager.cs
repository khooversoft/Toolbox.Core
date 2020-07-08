// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Actor type manager for lambda or activator creation
    /// </summary>
    public class ActorTypeManager
    {
        private readonly ConcurrentDictionary<Type, ActorTypeRegistration> _actorRegistration = new ConcurrentDictionary<Type, ActorTypeRegistration>();
        private readonly ILogger _logger;

        public ActorTypeManager(ILogger<ActorTypeManager> logger)
        {
            logger.VerifyNotNull(nameof(logger));

            _logger = logger;
        }

        /// <summary>
        /// Register actor for lambda creation
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="createImplementation">creation lambda</param>
        /// <returns>this</returns>
        public ActorTypeManager Register<T>(Func<T> createImplementation) where T : IActor
        {
            typeof(T).IsInterface.VerifyAssert(x => x == true, $"{typeof(T).FullName} must be an interface");

            IActor create() => createImplementation();

            _actorRegistration.AddOrUpdate(typeof(T), x => new ActorTypeRegistration(x, create), (x, r) => r);

            _logger.LogTrace($"lambda registered for type:{typeof(T)}");
            return this;
        }

        /// <summary>
        /// Register type based
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="actorTypeRegistration">actor type registration</param>
        /// <returns></returns>
        public ActorTypeManager Register(ActorTypeRegistration actorTypeRegistration)
        {
            actorTypeRegistration.VerifyNotNull(nameof(actorTypeRegistration));
            actorTypeRegistration.InterfaceType.IsInterface.VerifyAssert(x => x = true, $"{actorTypeRegistration.InterfaceType.FullName} must be an interface");

            _actorRegistration.AddOrUpdate(actorTypeRegistration.InterfaceType, actorTypeRegistration, (_, __) => actorTypeRegistration);

            _logger.LogTrace($"lambda registered for type:{actorTypeRegistration.InterfaceType.Name}");
            return this;
        }

        /// <summary>
        /// Create actor from either lambda or activator
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <param name="manager">actor manager</param>
        /// <returns>instance of actor implementation</returns>
        public T Create<T>(ActorKey actorKey, IActorManager manager) where T : IActor
        {
            actorKey.VerifyNotNull(nameof(actorKey));
            manager.VerifyNotNull(nameof(manager));

            typeof(T).IsInterface.VerifyAssert(x => x == true, $"{typeof(T)} must be an interface");

            Type actorType = typeof(T);

            ActorTypeRegistration? typeRegistration = GetTypeRegistration(actorType);

            if (typeRegistration == null)
            {
                var ex = new KeyNotFoundException($"Registration for {actorType.FullName} was not found");
                _logger.LogError(ex, "create failure");
                throw ex;
            }

            IActor actorObject = typeRegistration.CreateImplementation();

            // Set actor key and manager
            ActorBase? actorBase = actorObject as ActorBase;
            if (actorBase == null)
            {
                string failureMsg = $"Created actor type {actorObject.GetType()} does not derive from ActorBase";
                _logger.LogError(failureMsg);
                throw new InvalidOperationException(failureMsg);
            }

            actorBase.ActorKey = actorKey;
            actorBase.ActorManager = manager;
            actorBase.ActorType = actorType;

            return (T)actorObject;
        }

        private ActorTypeRegistration? GetTypeRegistration(Type actorType) =>
            _actorRegistration.TryGetValue(actorType, out ActorTypeRegistration typeRegistration) ? typeRegistration : null;
    }
}
