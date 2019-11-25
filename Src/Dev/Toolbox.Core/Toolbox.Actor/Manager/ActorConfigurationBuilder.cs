using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Builds the configuration for an actor manager
    /// </summary>
    public class ActorConfigurationBuilder
    {
        public ActorConfigurationBuilder()
        {
        }

        public ActorConfigurationBuilder(IActorConfiguration configuration)
        {
            configuration.Verify(nameof(configuration)).IsNotNull();

            Capacity = configuration.Capacity;
            ActorRepository = configuration.ActorRepository;
            ActorCallTimeout = configuration.ActorCallTimeout;
            ActorRetirementPeriod = configuration.ActorRetirementPeriod;
            InactivityScanPeriod = configuration.InactivityScanPeriod;
            Registration = configuration.Registration;
            WorkContext = configuration.WorkContext;
        }

        /// <summary>
        /// Maximum number of actors the manager can handle
        /// </summary>
        public int Capacity { get; private set; } = 10000;

        /// <summary>
        /// Repository for the actor types and instances
        /// </summary>
        public IActorRepository? ActorRepository { get; private set; }

        /// <summary>
        /// Timeout for actor calls
        /// </summary>
        public TimeSpan ActorCallTimeout { get; private set; } = TimeSpan.FromSeconds(120);

        /// <summary>
        /// Time span that inactive actors can be retried.  Each time an actor is accessed,
        /// its time stamp is updated.  Actors are collected with this time span is exceeded.
        /// </summary>
        public TimeSpan ActorRetirementPeriod { get; private set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// Schedule for when inactive actors are destroyed
        /// </summary>
        public TimeSpan InactivityScanPeriod { get; private set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Actor type registrations
        /// </summary>
        public IList<ActorTypeRegistration> Registration { get; } = new List<ActorTypeRegistration>();

        /// <summary>
        /// Work context for manager
        /// </summary>
        public IWorkContext? WorkContext { get; private set; }

        public ActorConfigurationBuilder Set(int capacity)
        {
            Capacity = capacity;
            return this;
        }

        public ActorConfigurationBuilder Set(IActorRepository repository)
        {
            ActorRepository = repository;
            return this;
        }

        public ActorConfigurationBuilder Set(IWorkContext context)
        {
            WorkContext = context;
            return this;
        }

        public ActorConfigurationBuilder Register<T>(Func<IWorkContext, T> createImplementation) where T : IActor
        {
            createImplementation.Verify(nameof(createImplementation)).IsNotNull();

            Registration.Add(new ActorTypeRegistration(typeof(T), x => createImplementation(x)));
            return this;
        }

        public ActorConfigurationBuilder SetActorCallTimeout(TimeSpan span)
        {
            ActorCallTimeout = span;
            return this;
        }

        public ActorConfigurationBuilder SetActorRetirementPeriod(TimeSpan span)
        {
            ActorRetirementPeriod = span;
            return this;
        }

        public ActorConfigurationBuilder SetInactivityScanPeriod(TimeSpan span)
        {
            InactivityScanPeriod = span;
            return this;
        }

        public IActorConfiguration Build()
        {
            return new ActorConfiguration(
                capacity: Capacity,
                actorRepository: ActorRepository!,
                actorCallTimeout: ActorCallTimeout,
                actorRetirementPeriod: ActorRetirementPeriod,
                inactivityScanPeriod: InactivityScanPeriod,
                registrations: Registration,
                workContext: WorkContext!
            );
        }
    }

    public static class ActorConfigurationBuilderExtensions
    {
        public static IActorManager ToActorManager(this IActorConfiguration self)
        {
            self.Verify(nameof(self)).IsNotNull();

            return new ActorManager(self);
        }
    }
}
