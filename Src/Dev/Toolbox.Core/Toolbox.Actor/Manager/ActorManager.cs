// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Actor manager handles organization and functional requirements for an actor environment.
    /// </summary>
    public class ActorManager : IActorManager, IDisposable
    {
        private const string _disposedTestText = "Actor Manager has been disposed";

        private readonly IActorRepository _actorRepository;
        private readonly ActorTypeManager _typeManager;
        private readonly object _lock = new object();
        private readonly ILogger<ActorManager> _logger;
        private int _disposed;

        public ActorManager(ActorConfiguration configuration, ILoggerFactory loggerFactory)
        {
            configuration.VerifyNotNull(nameof(configuration));
            loggerFactory.VerifyNotNull(nameof(loggerFactory));

            Configuration = configuration;
            _logger = loggerFactory.CreateLogger<ActorManager>();
            _actorRepository = new ActorRepository(Configuration, loggerFactory.CreateLogger<ActorRepository>());

            _typeManager = new ActorTypeManager(loggerFactory.CreateLogger<ActorTypeManager>());
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public ActorConfiguration Configuration { get; }

        /// <summary>
        /// Is actor manager running (not disposed)
        /// </summary>
        public bool IsRunning => _disposed == 0;

        /// <summary>
        /// Register actor and lambda creator
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="createImplementation">creator</param>
        /// <returns>this</returns>
        public IActorManager Register<T>(Func<T> createImplementation) where T : IActor
        {
            Verify.Assert(IsRunning, _disposedTestText);
            createImplementation.VerifyNotNull(nameof(createImplementation));

            _typeManager.Register(createImplementation);
            return this;
        }

        /// <summary>
        /// Does actor instance exist?
        /// </summary>
        /// <typeparam name="T">interface of actor</typeparam>
        /// <param name="actorKey">actor key</param>
        /// <returns></returns>
        public bool Exist<T>(ActorKey actorKey) where T : IActor
        {
            return _actorRepository.Lookup(typeof(T), actorKey) != null;
        }

        /// <summary>
        /// Create proxy to actor, return current instance or create one
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>actor proxy interface</returns>
        public T GetActor<T>(ActorKey actorKey) where T : IActor
        {
            Verify.Assert(IsRunning, _disposedTestText);

            actorKey.VerifyNotNull(nameof(actorKey));

            Type actorType = typeof(T);

            lock (_lock)
            {
                // Lookup instance of actor (type + actorKey)
                IActorRegistration? actorRegistration = _actorRepository.Lookup(actorType, actorKey);
                if (actorRegistration != null)
                {
                    return actorRegistration.GetInstance<T>();
                }

                // Create actor
                IActor actorObject = _typeManager.Create<T>(actorKey, this);

                IActorBase? actorBase = actorObject as IActorBase;
                if (actorBase == null)
                {
                    var ex = new ArgumentException($"Actor {actorObject.GetType().FullName} does not implement IActorBase");
                    _logger.LogError("Cannot create", ex);
                    throw ex;
                }

                // Create proxy
                T actorInterface = ActorProxy<T>.Create(actorBase, this);
                actorRegistration = new ActorRegistration(typeof(T), actorKey, actorBase, actorInterface);

                _actorRepository.Set(actorRegistration);

                // Create proxy for interface
                return actorRegistration.GetInstance<T>();
            }
        }

        /// <summary>
        /// Deactivate actor
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>true if deactivated, false if not found</returns>
        public async Task<bool> Deactivate<T>(ActorKey actorKey)
        {
            Verify.Assert(IsRunning, _disposedTestText);
            actorKey.VerifyNotNull(nameof(actorKey));

            IActorRegistration? actorRegistration = await _actorRepository.Remove(typeof(T), actorKey).ConfigureAwait(false);
            if (actorRegistration == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deactivate actor
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>true if deactivated, false if not found</returns>
        public async Task<bool> Deactivate(Type actorType, ActorKey actorKey)
        {
            Verify.Assert(IsRunning, _disposedTestText);
            actorType.VerifyNotNull(nameof(actorType));

            IActorRegistration? subject = await _actorRepository.Remove(actorType, actorKey).ConfigureAwait(false);
            if (subject == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deactivate all actors
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        public async Task DeactivateAll()
        {
            IsRunning.VerifyAssert(x => x == true, _disposedTestText);

            await _actorRepository.Clear().ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose of all resources, actors are deactivated, DI container is disposed
        /// </summary>
        public void Dispose()
        {
            int testDisposed = Interlocked.CompareExchange(ref _disposed, 1, 0);
            if (testDisposed == 0)
            {
                Task.Run(() => _actorRepository.Clear())
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
