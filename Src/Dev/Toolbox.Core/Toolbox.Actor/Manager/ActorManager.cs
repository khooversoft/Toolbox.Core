// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
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
        private static StringVector _tag = new StringVector(nameof(ActorManager));
        private static IWorkContext _actorManagerWorkContext = new WorkContextBuilder().Set(_tag).Build();

        private readonly IActorRepository _actorRepository;
        private readonly ActorTypeManager _typeManager = new ActorTypeManager();
        private int _disposed;
        private bool _disposing = false;
        private const string _disposedTestText = "Actor Manager has been disposed";

        public ActorManager()
            : this(ActorConfiguration.Default)
        {
        }

        public ActorManager(IActorConfiguration configuration)
        {
            configuration.Verify(nameof(configuration)).IsNotNull();

            Configuration = configuration;
            _actorRepository = Configuration.ActorRepository ?? new ActorRepository(Configuration);

            foreach (ActorTypeRegistration registration in configuration?.Registration ?? Enumerable.Empty<ActorTypeRegistration>())
            {
                _typeManager.Register(_actorManagerWorkContext, registration);
            }
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IActorConfiguration Configuration { get; private set; }

        /// <summary>
        /// Is actor manager running (not disposed)
        /// </summary>
        public bool IsRunning { get { return _disposed == 0 || _disposing; } }

        /// <summary>
        /// Register actor and lambda creator
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="createImplementation">creator</param>
        /// <returns>this</returns>
        public IActorManager Register<T>(IWorkContext context, Func<IWorkContext, T> createImplementation) where T : IActor
        {
            Verify.Assert(IsRunning, _disposedTestText);
            context.Verify(nameof(context)).IsNotNull();
            createImplementation.Verify(nameof(createImplementation)).IsNotNull();

            _typeManager.Register<T>(context.With(_tag), createImplementation);
            return this;
        }

        /// <summary>
        /// Create proxy to actor, return current instance or create one
        /// </summary>
        /// <typeparam name="T">actor interface </typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns></returns>
        public Task<T> CreateProxy<T>(IWorkContext context, string actorKey) where T : IActor
        {
            return CreateProxy<T>(context, new ActorKey(actorKey));
        }

        /// <summary>
        /// Create proxy to actor, return current instance or create one
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>actor proxy interface</returns>
        public async Task<T> CreateProxy<T>(IWorkContext context, ActorKey actorKey) where T : IActor
        {
            Verify.Assert(IsRunning, _disposedTestText);

            context.Verify(nameof(context)).IsNotNull();
            context.Verify(nameof(context)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();
            context = context.With(_tag);

            Type actorType = typeof(T);

            // Lookup instance of actor (type + actorKey)
            IActorRegistration? actorRegistration = _actorRepository.Lookup(actorType, actorKey);
            if (actorRegistration != null)
            {
                return actorRegistration.GetInstance<T>();
            }

            // Create actor
            IActor actorObject = _typeManager.Create<T>(context, actorKey, this);

            IActorBase? actorBase = actorObject as IActorBase;
            if (actorBase == null)
            {
                var ex = new ArgumentException($"Actor {actorObject.GetType().FullName} does not implement IActorBase");
                Configuration.WorkContext.Telemetry.Error(context, "Cannot create", ex);
                throw ex;
            }

            // Create proxy
            T actorInterface = ActorProxy<T>.Create(context, actorBase, this);
            actorRegistration = new ActorRegistration(typeof(T), actorKey, actorBase, actorInterface);

            await _actorRepository.Set(context, actorRegistration).ConfigureAwait(false);

            // Create proxy for interface
            return actorRegistration.GetInstance<T>();
        }

        /// <summary>
        /// Deactivate actor
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>true if deactivated, false if not found</returns>
        public async Task<bool> Deactivate<T>(IWorkContext context, ActorKey actorKey)
        {
            Verify.Assert(IsRunning, _disposedTestText);
            context.Verify(nameof(context)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();

            IActorRegistration? actorRegistration = await _actorRepository.Remove(context, typeof(T), actorKey).ConfigureAwait(false);
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
        public async Task<bool> Deactivate(IWorkContext context, Type actorType, ActorKey actorKey)
        {
            Verify.Assert(IsRunning, _disposedTestText);
            context.Verify(nameof(context)).IsNotNull();
            actorType.Verify(nameof(actorType)).IsNotNull();

            IActorRegistration? subject = await _actorRepository.Remove(context, actorType, actorKey).ConfigureAwait(false);
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
        public async Task DeactivateAll(IWorkContext context)
        {
            IsRunning.Verify().Assert(x => x == true, _disposedTestText);
            context.Verify(nameof(context)).IsNotNull();

            await _actorRepository.Clear(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose of all resources, actors are deactivated, DI container is disposed
        /// </summary>
        public void Dispose()
        {
            try
            {
                _disposing = true;
                int testDisposed = Interlocked.CompareExchange(ref _disposed, 1, 0);
                if (testDisposed == 0)
                {
                    Task.Run(() => _actorRepository.Clear(_actorManagerWorkContext))
                        .ConfigureAwait(false)
                        .GetAwaiter();
                }
            }
            finally
            {
                _disposing = false;
            }
        }
    }
}
