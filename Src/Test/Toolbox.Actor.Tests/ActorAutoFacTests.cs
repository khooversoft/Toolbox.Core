// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using Khooversoft.Toolbox.Actor;
using System.Threading.Tasks;
using Xunit;
using Autofac;
using Khooversoft.Toolbox.Autofac;

namespace Toolbox.Actor.Tests
{
    [Trait("Category", "Actor")]
    public class ActorAutoFacTests
    {
        private IWorkContext _context = WorkContext.Empty;

        [Fact]
        public async Task GivenAutofac_WhenProxyCreated_ShouldPass()
        {
            IActorManager manager = new ActorManager();

            var builder = new ContainerBuilder();
            builder.RegisterType<StringCache>().As<ICache>();
            builder.RegisterInstance(manager).As<IActorManager>();
            IContainer container = builder.Build();

            using (container.BeginLifetimeScope())
            {
                manager.Register<ICache>(_ => container.Resolve<ICache>());

                ActorKey key = new ActorKey("cache/test");
                ICache cache = await manager.CreateProxy<ICache>(key);

                (await cache.GetCount()).Should().Be(1);
                await manager.Deactivate<ICache>(key);
                (await cache.GetCount()).Should().Be(2);
            }

            await manager.DeactivateAll();
        }

        [Fact]
        public async Task GivenAutofac_ConstructedFromBuilder_ShouldPass()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<StringCache>().As<ICache>();
            ILifetimeScope container = builder.Build();

            IActorManager? manager = new ActorConfigurationBuilder()
                .Register(_ => container.Resolve<ICache>())
                .Build()
                .ToActorManager();

            using (container.BeginLifetimeScope())
            {
                ActorKey key = new ActorKey("cache/test");
                ICache cache = await manager.CreateProxy<ICache>(key);

                (await cache.GetCount()).Should().Be(1);
                await manager.Deactivate<ICache>(key);
                (await cache.GetCount()).Should().Be(2);
            }

            await manager.DeactivateAll();
        }

        [Fact]
        public async Task GivenAutofacOnlyRegistration_WithResolveOptional_ShouldPass()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<StringCache>().As<ICache>();
            ILifetimeScope container = builder.Build();

            IWorkContext context = new WorkContextBuilder()
                .Set(new ServiceContainerBuilder().SetLifetimeScope(container).Build())
                .Build();

            IActorManager? manager = new ActorConfigurationBuilder()
                .Set(context)
                .Build()
                .ToActorManager();

            using (container.BeginLifetimeScope())
            {
                ActorKey key = new ActorKey("cache/test");
                ICache cache = await manager.CreateProxy<ICache>(key);

                (await cache.GetCount()).Should().Be(1);
                await manager.Deactivate<ICache>(key);
                (await cache.GetCount()).Should().Be(2);
            }

            await manager.DeactivateAll();
        }

        private interface ICache : IActor
        {
            Task<int> GetCount();
        }

        private class StringCache : ActorBase, ICache
        {
            private int _count = 0;

            public StringCache()
            {
            }

            protected override Task OnActivate(IWorkContext context)
            {
                _count++;
                return base.OnActivate(context);
            }

            protected override Task OnDeactivate(IWorkContext context)
            {
                _count++;
                return base.OnDeactivate(context);
            }

            public Task<int> GetCount()
            {
                return Task.FromResult(_count);
            }
        }
    }
}
