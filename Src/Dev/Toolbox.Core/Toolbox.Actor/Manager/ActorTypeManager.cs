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
        private readonly StringVector _tag = new StringVector(nameof(ActorManager));

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
            context.Verify(nameof(context)).IsNotNull();
            typeof(T).IsInterface.Verify().Assert(x => x = true, $"{typeof(T).FullName} must be an interface");
            context = context.With(_tag);

            IActor create(IWorkContext c) => createImplementation(c);

            _actorRegistration.AddOrUpdate(typeof(T), x => new ActorTypeRegistration(x, create), (x, r) => r);

            context.Telemetry.Verbose(context.With(_tag), $"lambda registered for type:{typeof(T)}");
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
            context.Verify(nameof(context)).IsNotNull();
            actorTypeRegistration.Verify(nameof(actorTypeRegistration)).IsNotNull();
            actorTypeRegistration.InterfaceType.IsInterface.Verify().Assert(x => x = true, $"{actorTypeRegistration.InterfaceType.FullName} must be an interface");
            context = context.With(_tag);

            _actorRegistration.AddOrUpdate(actorTypeRegistration.InterfaceType, actorTypeRegistration, (_, __) => actorTypeRegistration);

            context.Telemetry.Verbose(context.With(_tag), $"lambda registered for type:{actorTypeRegistration.InterfaceType.Name}");
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
            context.Verify(nameof(context)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();
            manager.Verify(nameof(manager)).IsNotNull();

            typeof(T).IsInterface.Verify().Assert(x => x == true, $"{typeof(T)} must be an interface");
            context = context.With(_tag);

            Type actorType = typeof(T);

            ActorTypeRegistration? typeRegistration = GetTypeRegistration(actorType) ?? GetTypeFromDi(context, actorType);

            if (typeRegistration == null)
            {
                var ex = new KeyNotFoundException($"Registration for {actorType.FullName} was not found");
                context.Telemetry.Error(context.With(_tag), "create failure", ex);
                throw ex;
            }

            IActor actorObject = typeRegistration.CreateImplementation(context);

            // Set actor key and manager
            ActorBase? actorBase = actorObject as ActorBase;
            if( actorBase == null)
            {
                string failureMsg = $"Created actor type {actorObject.GetType()} does not derive from ActorBase";
                context.Telemetry.Error(context.With(_tag), failureMsg);
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
            IServiceProviderProxy? serviceProviderProxy = context.Container as IServiceProviderProxy;
            if (serviceProviderProxy == null) return null;

            var actor = serviceProviderProxy.GetServiceOptional(actorType);
            if (actor == null) return null;

            var registration = new ActorTypeRegistration(actorType, x => (IActor)actor);

            _actorRegistration.AddOrUpdate(registration.InterfaceType, registration, (_, __) => registration);
            return registration;
        }
    }
}
