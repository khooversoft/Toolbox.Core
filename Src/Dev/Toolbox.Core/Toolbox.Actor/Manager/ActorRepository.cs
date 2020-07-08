// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly LruCache<RegistrationKey, IActorRegistration> _actorCache;
        private readonly ActionBlock<IActorRegistration> _actorRemove;
        private readonly object _lock = new object();
        private int _timerLockValue;
        private readonly Timer _timer;
        private readonly ActorConfiguration _configuration;
        private readonly ILogger<ActorRepository> _logger;

        public ActorRepository(ActorConfiguration configuration, ILogger<ActorRepository> logger)
        {
            configuration.VerifyNotNull(nameof(configuration));
            configuration.Capacity.VerifyAssert(x => x > 0, $"Capacity {configuration.Capacity} must be greater than 0");
            logger.VerifyNotNull(nameof(logger));

            _actorRemove = new ActionBlock<IActorRegistration>(x => RetireActor(x));
            _configuration = configuration;
            _logger = logger;
            _actorCache = new LruCache<RegistrationKey, IActorRegistration>(_configuration.Capacity, new RegistrationKeyComparer());
            _actorCache.CacheItemRemoved += x => _actorRemove.Post(x.Value);

            _timer = new Timer(GarbageCollection, null, _configuration.InactivityScanPeriod, _configuration.InactivityScanPeriod);
        }

        /// <summary>
        /// Clear all actors from the system.  Each active actor will be deactivated
        /// </summary>
        /// <param name="context">context</param>
        /// <returns></returns>
        public Task Clear()
        {
            _logger.LogTrace("Clearing actor container");
            List<IActorRegistration> list;

            lock (_lock)
            {
                list = new List<IActorRegistration>(_actorCache.GetValues());
                _actorCache.Clear();
            }

            return list
                .Select(x => x.Instance.Deactivate())
                .WhenAll();
        }

        /// <summary>
        /// Set actor (add or replace)
        /// </summary>
        /// <param name="registration">actor registration</param>
        /// <returns>task</returns>
        public void Set(IActorRegistration registration)
        {
            registration.VerifyNotNull(nameof(registration));

            _logger.LogTrace($"Setting actor {registration.ActorKey}");
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

            // Dispose of the old actor
            if (currentActorRegistration != null) _actorRemove.Post(currentActorRegistration);

            registration.Instance!.Activate().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Lookup actor
        /// </summary>
        /// <param name="actorType">actor type</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>actor registration or null if not exist</returns>
        public IActorRegistration? Lookup(Type actorType, ActorKey actorKey)
        {
            actorType.VerifyNotNull(nameof(actorType));
            actorKey.VerifyNotNull(nameof(actorKey));

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
        public async Task<IActorRegistration?> Remove(Type actorType, ActorKey actorKey)
        {
            actorType.VerifyNotNull(nameof(actorType));
            actorKey.VerifyNotNull(nameof(actorKey));

            _logger.LogTrace($"Removing actor {actorKey}");

            IActorRegistration registration;

            lock (_lock)
            {
                var key = new RegistrationKey(actorType, actorKey.Key);
                if (!_actorCache.TryRemove(key, out registration)) return null;
            }

            try
            {
                await registration.Instance.Deactivate();
            }
            finally
            {
                registration.Instance.Dispose();
            }

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
                _logger.LogTrace("GarbageCollection");
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

            await Remove(actorRegistration.ActorType, actorRegistration.ActorKey);
        }

        /// <summary>
        /// Registration key so we can use a single dictionary
        /// </summary>
        private class RegistrationKey
        {
            public RegistrationKey(Type type, Guid key)
            {
                type.VerifyNotNull(nameof(type));

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
