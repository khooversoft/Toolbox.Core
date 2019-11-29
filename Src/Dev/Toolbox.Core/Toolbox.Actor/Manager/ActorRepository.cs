// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Stores actor registrations.  When an actor registration is added or removed, the actor's activate and deactivate methods are called
    /// </summary>
    public class ActorRepository : IActorRepository
    {
        private readonly IWorkContext _workContext;
        private readonly StringVector _tag = new StringVector(nameof(ActorRepository));

        private readonly LruCache<RegistrationKey, IActorRegistration> _actorCache;
        private readonly ActionBlock<IActorRegistration> _actorRemove;
        private readonly object _lock = new object();
        private int _timerLockValue;
        private readonly Timer _timer;
        private readonly ActorConfiguration _configuration;

        public ActorRepository(ActorConfiguration configuration)
        {
            configuration.Verify(nameof(configuration)).IsNotNull();
            configuration.Capacity.Verify().Assert(x => x > 0, $"Capacity {configuration.Capacity} must be greater than 0");

            _actorRemove = new ActionBlock<IActorRegistration>(x => RetireActor(x));

            _workContext = new WorkContextBuilder(configuration.WorkContext)
                .Set(_tag)
                .Set(new CorrelationVector("Khooversoft-ActorRepository"))
                .Build();

            _configuration = configuration;

            _actorCache = new LruCache<RegistrationKey, IActorRegistration>(_configuration.Capacity, new RegistrationKeyComparer());
            _actorCache.CacheItemRemoved += x => _actorRemove.Post(x.Value);

            _timer = new Timer(GarbageCollection, null, _configuration.InactivityScanPeriod, _configuration.InactivityScanPeriod);
        }

        /// <summary>
        /// Clear all actors from the system.  Each active actor will be deactivated
        /// </summary>
        /// <param name="context">context</param>
        /// <returns></returns>
        public async Task Clear(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            context = context.With(_tag);

            context.Telemetry.Verbose(context, "Clearing actor container");
            List<IActorRegistration> list;

            lock (_lock)
            {
                list = new List<IActorRegistration>(_actorCache.Values);
                _actorCache.Clear();
            }

            foreach (var item in list)
            {
                await item.Instance.Deactivate(context).ConfigureAwait(false);
                item.Instance.Dispose();
            }
        }

        /// <summary>
        /// Set actor (add or replace)
        /// </summary>
        /// <param name="registration">actor registration</param>
        /// <returns>task</returns>
        public async Task Set(IWorkContext context, IActorRegistration registration)
        {
            registration.Verify(nameof(registration)).IsNotNull();
            context = context.With(_tag);

            context.Telemetry.Verbose(context, $"Setting actor {registration.ActorKey}");
            IActorRegistration? currentActorRegistration = null;

            var key = new RegistrationKey(registration.ActorType, registration.ActorKey.Key);

            lock (_lock)
            {
                if (!_actorCache.TryRemove(key, out currentActorRegistration))
                {
                    currentActorRegistration = null;
                }

                _actorCache.Set(key, registration);
            }

            if (currentActorRegistration != null)
            {
                await currentActorRegistration.Instance!.Deactivate(context).ConfigureAwait(false);
                currentActorRegistration.Instance.Dispose();
            }

            await registration.Instance!.Activate(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Lookup actor
        /// </summary>
        /// <param name="actorType">actor type</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>actor registration or null if not exist</returns>
        public IActorRegistration? Lookup(Type actorType, ActorKey actorKey)
        {
            actorType.Verify(nameof(actorType)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();

            lock (_lock)
            {
                var key = new RegistrationKey(actorType, actorKey.Key);

                if (_actorCache.TryGetValue(key, out IActorRegistration registration)) return registration;

                return null;
            }
        }

        /// <summary>
        /// Remove actor from container
        /// </summary>
        /// <param name="actorType">actor type</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>actor registration or null if not exist</returns>
        public async Task<IActorRegistration?> Remove(IWorkContext context, Type actorType, ActorKey actorKey)
        {
            actorType.Verify(nameof(actorType)).IsNotNull();
            actorKey.Verify(nameof(actorKey)).IsNotNull();
            context = context.With(_tag);

            context.Telemetry.Verbose(context.With(_tag), $"Removing actor {actorKey}");

            IActorRegistration registration;

            lock (_lock)
            {
                var key = new RegistrationKey(actorType, actorKey.Key);
                if (!_actorCache.TryRemove(key, out registration)) return null;
            }

            await registration.Instance.Deactivate(context).ConfigureAwait(false);
            registration.Instance.Dispose();
            return registration;
        }

        /// <summary>
        /// Scan current actors for inactivity equal or greater than retirement period
        /// </summary>
        /// <param name="obj">not used</param>
        private void GarbageCollection(object obj)
        {
            int currentValue = Interlocked.CompareExchange(ref _timerLockValue, 1, 0);
            if (currentValue != 0) return;

            try
            {
                _workContext.Telemetry.Verbose(_workContext.With(_tag), "GarbageCollection");
                DateTimeOffset retireDate = DateTimeOffset.UtcNow.AddSeconds(-_configuration.ActorRetirementPeriod.TotalSeconds);

                foreach (var item in _actorCache)
                {
                    // Check if actor is active or last access is after the retire date
                    if (!item.Value.Instance.Active || item.LastAccessed < retireDate)
                    {
                        _actorRemove.Post(item.Value);
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref _timerLockValue, 0);
            }
        }

        /// <summary>
        /// Loop through remove queue and remove actors
        /// </summary>
        /// <returns>task</returns>
        private async Task RetireActor(IActorRegistration actorRegistration)
        {
            var key = new RegistrationKey(actorRegistration.ActorType, actorRegistration.ActorKey.Key);
            _actorCache.Remove(key);

            await Remove(_workContext, actorRegistration.ActorType, actorRegistration.ActorKey).ConfigureAwait(false);
        }

        /// <summary>
        /// Registration key so we can use a single dictionary
        /// </summary>
        private class RegistrationKey
        {
            public RegistrationKey(Type type, Guid key)
            {
                type.Verify(nameof(type)).IsNotNull();

                Type = type;
                Key = key;
            }

            public Type Type { get; }

            public Guid Key { get; }
        }

        /// <summary>
        /// Key compare
        /// </summary>
        private class RegistrationKeyComparer : IEqualityComparer<RegistrationKey>
        {
            public bool Equals(RegistrationKey x, RegistrationKey y)
            {
                return x.Type == y.Type &&
                    x.Key == y.Key;
            }

            public int GetHashCode(RegistrationKey obj)
            {
                return obj.Type.GetHashCode() ^ obj.Key.GetHashCode();
            }
        }
    }
}
