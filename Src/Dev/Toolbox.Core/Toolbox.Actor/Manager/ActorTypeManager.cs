// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
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

        public ActorTypeManager()
        {
        }

        /// <summary>
        /// Register actor for lambda creation
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="createImplementation">creation lambda</param>
        /// <returns>this</returns>
        public ActorTypeManager Register<T>(IWorkContext context, Func<IWorkContext, T> createImplementation) where T : IActor
        {
            context.VerifyNotNull(nameof(context));
            typeof(T).IsInterface.VerifyAssert(x => x = true, $"{typeof(T).FullName} must be an interface");

            IActor create(IWorkContext c) => createImplementation(c);

            _actorRegistration.AddOrUpdate(typeof(T), x => new ActorTypeRegistration(x, create), (x, r) => r);

            context.Telemetry.Verbose(context, $"lambda registered for type:{typeof(T)}");
            return this;
        }

        /// <summary>
        /// Register type based
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="actorTypeRegistration">actor type registration</param>
        /// <returns></returns>
        public ActorTypeManager Register(IWorkContext context, ActorTypeRegistration actorTypeRegistration)
        {
            context.VerifyNotNull(nameof(context));
            actorTypeRegistration.VerifyNotNull(nameof(actorTypeRegistration));
            actorTypeRegistration.InterfaceType.IsInterface.VerifyAssert(x => x = true, $"{actorTypeRegistration.InterfaceType.FullName} must be an interface");

            _actorRegistration.AddOrUpdate(actorTypeRegistration.InterfaceType, actorTypeRegistration, (_, __) => actorTypeRegistration);

            context.Telemetry.Verbose(context, $"lambda registered for type:{actorTypeRegistration.InterfaceType.Name}");
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
        public T Create<T>(IWorkContext context, ActorKey actorKey, IActorManager manager) where T : IActor
        {
            context.VerifyNotNull(nameof(context));
            actorKey.VerifyNotNull(nameof(actorKey));
            manager.VerifyNotNull(nameof(manager));
            context = context.WithActivity();

            typeof(T).IsInterface.VerifyAssert(x => x == true, $"{typeof(T)} must be an interface");

            Type actorType = typeof(T);

            ActorTypeRegistration? typeRegistration = GetTypeRegistration(actorType) ?? GetTypeFromDi(context, actorType);

            if (typeRegistration == null)
            {
                var ex = new KeyNotFoundException($"Registration for {actorType.FullName} was not found");
                context.Telemetry.Error(context, "create failure", ex);
                throw ex;
            }

            IActor actorObject = typeRegistration.CreateImplementation(context);

            // Set actor key and manager
            ActorBase? actorBase = actorObject as ActorBase;
            if (actorBase == null)
            {
                string failureMsg = $"Created actor type {actorObject.GetType()} does not derive from ActorBase";
                context.Telemetry.Error(context, failureMsg);
                throw new InvalidOperationException(failureMsg);
            }

            actorBase.ActorKey = actorKey;
            actorBase.ActorManager = manager;
            actorBase.ActorType = actorType;

            return (T)actorObject;
        }

        private ActorTypeRegistration? GetTypeRegistration(Type actorType)
        {
            return _actorRegistration.TryGetValue(actorType, out ActorTypeRegistration typeRegistration) ? typeRegistration : null;
        }

        private ActorTypeRegistration? GetTypeFromDi(IWorkContext context, Type actorType)
        {
            IServiceContainer? serviceProviderProxy = context.Container as IServiceContainer;
            if (serviceProviderProxy == null) return null;

            var actor = serviceProviderProxy.GetServiceOptional(actorType);
            if (actor == null) return null;

            var registration = new ActorTypeRegistration(actorType, x => (IActor)actor);

            _actorRegistration.AddOrUpdate(registration.InterfaceType, registration, (_, __) => registration);
            return registration;
        }
    }
}
