﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;
using System.Threading;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Actor registration information
    /// </summary>
    public class ActorRegistration : IActorRegistration
    {
        private IActorBase? _instance;
        private IActor? _actorProxy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actorType">actor type</param>
        /// <param name="actorKey">actor key</param>
        /// <param name="instance">instance of the actor class</param>
        /// <param name="actorProxy">actor proxy</param>
        public ActorRegistration(Type actorType, ActorKey actorKey, IActorBase instance, IActor actorProxy)
        {
            actorType.Verify(nameof(actorType)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();
            instance.Verify(nameof(instance)).IsNotNull();
            actorProxy.Verify(nameof(actorProxy)).IsNotNull();

            ActorType = actorType;
            ActorKey = actorKey;
            _instance = instance;
            _actorProxy = actorProxy;
        }

        /// <summary>
        /// Actor key
        /// </summary>
        public ActorKey ActorKey { get; }

        /// <summary>
        /// Actor instance
        /// </summary>
        public IActorBase Instance => _instance ?? throw new InvalidOperationException("Actor Registration is Disposed");

        /// <summary>
        /// Type of actor
        /// </summary>
        public Type ActorType { get; }

        /// <summary>
        /// Proxy to actor
        /// </summary>
        public IActor ActorProxy => _actorProxy ?? throw new InvalidOperationException("Actor Registration is disposed");

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Interlocked.Exchange(ref _actorProxy, null);

            IActorBase? current = Interlocked.Exchange(ref _instance, null);
            current?.Dispose();
        }

        /// <summary>
        /// Get instance
        /// </summary>
        /// <typeparam name="T">actor type</typeparam>
        /// <returns>instance of actor</returns>
        public T GetInstance<T>() where T : IActor
        {
            return (T)ActorProxy;
        }
    }
}
