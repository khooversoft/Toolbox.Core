// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using Khooversoft.Toolbox.Actor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Toolbox.Actor.Tests
{
    [Trait("Category", "Actor")]
    public class ActorTests
    {
        private IWorkContext _context = WorkContext.Empty;

        [Fact]
        public async Task GivenActor_WhenCreated_KeyAndManagerShouldBeSet()
        {
            int count = 0;

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => count += y));

            ActorKey key = new ActorKey("cache/test");
            ICache cache = await manager.GetActor<ICache>(key);
            cache.GetActorKey().Should().Be(key);
            cache.GetActorManager().Should().Be(manager);

            count.Should().Be(1);
            (await manager.Deactivate<ICache>(key)).Should().BeTrue();
            count.Should().Be(0);

            await manager.DeactivateAll();
            count.Should().Be(0);
        }

        [Fact]
        public async Task GivenActor_WhenMultipleCreated_KeyAndManagerShouldBeSet()
        {
            int count = 0;

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => count += y));

            const int max = 10;
            var keyList = new List<ActorKey>();

            await Enumerable.Range(0, max)
                .ForEachAsync(async (x, index) =>
                {
                    ActorKey key = new ActorKey($"cache/test_{index}");
                    keyList.Add(key);

                    ICache cache = await manager.GetActor<ICache>(key);
                    cache.GetActorKey().Should().Be(key);
                    cache.GetActorManager().Should().Be(manager);
                });

            count.Should().Be(max);

            await keyList
                .ForEachAsync(async x =>
                {
                    ICache cache = await manager.GetActor<ICache>(x);
                    cache.GetActorKey().Should().Be(x);
                    cache.GetActorManager().Should().Be(manager);
                });

            count.Should().Be(max);

            await keyList
                .ForEachAsync(async x =>
                {
                    (await manager.Deactivate<ICache>(x)).Should().BeTrue();
                });

            count.Should().Be(0);

            await manager.DeactivateAll();
            count.Should().Be(0);
        }

        [Fact]
        public async Task GivenActor_WhenCreatedDeactivated_CountsShouldFollowLifecycle()
        {
            int count = 0;

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => count += y));

            ActorKey key = new ActorKey("cache/test");
            ICache cache = await manager.GetActor<ICache>(key);

            count.Should().Be(1);
            (await manager.Deactivate<ICache>(key)).Should().BeTrue();
            count.Should().Be(0);

            await manager.DeactivateAll();
            count.Should().Be(0);
        }

        [Fact]
        public async Task GivenActor_WhenDeactivatedAll_ActorCountShouldBeZero()
        {
            int count = 0;

            var manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => count += y));

            ActorKey key = new ActorKey("cache/test");
            ICache cache = await manager.GetActor<ICache>(key);

            count.Should().Be(1);
            await manager.DeactivateAll();

            count.Should().Be(0);
        }

        [Fact]
        public async Task ActorMultipleTest()
        {
            int count = 0;

            ActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => count += y));

            ActorKey key1 = new ActorKey("Cache/Test1");
            ICache cache1 = await manager.GetActor<ICache>(key1);
            count.Should().Be(1);

            ActorKey key2 = new ActorKey("Cache/Test2");
            ICache cache2 = await manager.GetActor<ICache>(key2);
            count.Should().Be(2);

            (await manager.Deactivate<ICache>(key1)).Should().BeTrue();
            count.Should().Be(1);

            (await manager.Deactivate<ICache>(key2)).Should().BeTrue();
            count.Should().Be(0);

            await manager.DeactivateAll();
        }

        [Fact]
        public async Task ActorMethodTest()
        {
            int count = 0;

            ActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => count += y));

            ActorKey key1 = new ActorKey("Cache/Test1");
            ICache cache1 = await manager.GetActor<ICache>(key1);
            count.Should().Be(1);

            const string firstText = "first";

            bool test = await cache1.IsCached(firstText);
            test.Should().BeFalse();
            await cache1.Add(firstText);
            test = await cache1.IsCached(firstText);
            test.Should().BeTrue();

            (await manager.Deactivate<ICache>(key1)).Should().BeTrue(); ;
            count.Should().Be(0);

            await manager.DeactivateAll();
        }

        [Fact]
        public async Task ActorSameInstanceTest()
        {
            int count = 0;

            var manager = new ActorManager();

            manager.Register<ICache>(_ => new StringCache(y => count += y));

            ActorKey key1 = new ActorKey("Cache/Test1");
            ICache cache1 = await manager.GetActor<ICache>(key1);
            count.Should().Be(1);

            ActorKey key2 = new ActorKey("Cache/Test2");
            ICache cache2 = await manager.GetActor<ICache>(key2);
            count.Should().Be(2);

            const string firstText = "first";
            const string secondText = "secondFirst";

            await cache1.Add(firstText);
            bool test = await cache1.IsCached(firstText);
            test.Should().BeTrue();

            await cache2.Add(secondText);
            bool test2 = await cache2.IsCached(secondText);
            test2.Should().BeTrue();

            ICache cache1Dup = await manager.GetActor<ICache>(key1);
            test = await cache1Dup.IsCached(firstText);
            test.Should().BeTrue();
            test = await cache1Dup.IsCached(secondText);
            test.Should().BeFalse();

            (await manager.Deactivate<ICache>(key1)).Should().BeTrue();
            (await manager.Deactivate<ICache>(key2)).Should().BeTrue();
            count.Should().Be(0);

            await manager.DeactivateAll();
        }

        private interface ICache : IActor
        {
            Task<bool> IsCached(string key);

            Task Add(string key);

            ActorKey GetActorKey();

            IActorManager GetActorManager();
        }

        private class StringCache : ActorBase, ICache
        {
            private HashSet<string> _values = new HashSet<string>();

            public StringCache(Action<int> increment)
            {
                Increment = increment;
            }

            private Action<int> Increment { get; }

            public ActorKey GetActorKey() => base.ActorKey;

            public IActorManager GetActorManager() => base.ActorManager;

            protected override Task OnActivate(IWorkContext context)
            {
                Increment(1);
                return base.OnActivate(context);
            }

            protected override Task OnDeactivate(IWorkContext context)
            {
                Increment(-1);
                return base.OnDeactivate(context);
            }

            public Task<bool> IsCached(string key)
            {
                return Task.FromResult(_values.Contains(key));
            }

            public Task Add(string key)
            {
                _values.Add(key);
                return Task.FromResult(0);
            }
        }
    }
}