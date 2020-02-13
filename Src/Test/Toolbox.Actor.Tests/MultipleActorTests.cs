// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Toolbox.Actor.Tests
{
    public class MultipleActorTests
    {
        private readonly ITestOutputHelper _output;

        public MultipleActorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Given2Actors_WhenCreatedAndDeleted_ShouldPass()
        {
            int count = 0;
            int count2 = 0;
            const int max = 10;

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => CountControl(ref count, y)));
            manager.Register<ICache2>(_ => new StringCache2(y => CountControl(ref count2, y)));

            Enumerable.Range(0, max)
                .ForEach(x =>
                {
                    ActorKey key = new ActorKey($"cache/test/{x}");
                    ICache cache = manager.GetActor<ICache>(key);
                    cache.GetActorKey().Should().Be(key);
                    cache.GetActorManager().Should().Be(manager);

                    ActorKey key2 = new ActorKey($"cache/test/{x}");
                    ICache2 cache2 = manager.GetActor<ICache2>(key2);
                    cache2.GetActorKey().Should().Be(key2);
                    cache2.GetActorManager().Should().Be(manager);
                });

            count.Should().Be(max);
            count2.Should().Be(max);

            await Enumerable.Range(0, max)
                .Select(async x =>
                {
                    ActorKey key = new ActorKey($"cache/test/{x}");
                    (await manager.Deactivate<ICache>(key)).Should().BeTrue();

                    ActorKey key2 = new ActorKey($"cache/test/{x}");
                    (await manager.Deactivate<ICache2>(key2)).Should().BeTrue();
                })
                .WhenAll();

            count.Should().Be(0);
            count2.Should().Be(0);

            await manager.DeactivateAll();
            count.Should().Be(0);
            count2.Should().Be(0);
        }

        [Fact]
        public async Task Given2Actors_WhenCreatedAndDeletedDifferentTask_ShouldPass()
        {
            int count = 0;
            int count2 = 0;
            const int max = 10;
            const int maxLoop = 10;

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => CountControl(ref count, y)));
            manager.Register<ICache2>(_ => new StringCache2(y => CountControl(ref count2, y)));

            for (int loop = 0; loop < maxLoop; loop++)
            {
                _output.WriteLine($"Loop: {loop}");

                await Enumerable.Range(0, max)
                    .Select(x => new Task[]
                    {
                        Task.Run(() => {
                            ActorKey key = new ActorKey($"cache/test/{x}");
                            ICache cache = manager.GetActor<ICache>(key);
                            cache.GetActorKey().Should().Be(key);
                            cache.GetActorManager().Should().Be(manager);
                        }),
                        Task.Run(() => {
                            ActorKey key2 = new ActorKey($"cache/test/{x}");
                            ICache2 cache2 = manager.GetActor<ICache2>(key2);
                            cache2.GetActorKey().Should().Be(key2);
                            cache2.GetActorManager().Should().Be(manager);
                        }),
                    })
                    .SelectMany(x => x)
                    .WhenAll();

                count.Should().Be(max);
                count2.Should().Be(max);

                await Enumerable.Range(0, max)
                    .Select(x => new Task<bool>[]
                    {
                        Task.Run(async () => await manager.Deactivate<ICache>(new ActorKey($"cache/test/{x}"))),
                        Task.Run(async () => await manager.Deactivate<ICache2>(new ActorKey($"cache/test/{x}"))),
                    })
                    .SelectMany(x => x)
                    .WhenAll();

                count.Should().Be(0);
                count2.Should().Be(0);

                await manager.DeactivateAll();
                count.Should().Be(0);
                count2.Should().Be(0);
            }
        }

        [Fact]
        public async Task Given2Actors_WhenCreatedAndDeletedDifferentKeyRange_ShouldPass()
        {
            int count = 0;
            int count2 = 0;
            const int max = 1000;
            const int maxLoop = 10;

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => CountControl(ref count, y)));
            manager.Register<ICache2>(_ => new StringCache2(y => CountControl(ref count2, y)));

            for (int loop = 0; loop < maxLoop; loop++)
            {
                _output.WriteLine($"Loop: {loop}");

                await Enumerable.Range(0, max)
                    .Select((x, i) => new Task[]
                    {
                        Task.Run(() => manager.GetActor<ICache>(new ActorKey($"cache/test/{i}"))),
                        Task.Run(() => manager.GetActor<ICache2>(new ActorKey($"cache/test/{i+100}"))),
                    })
                    .SelectMany(x => x)
                    .WhenAll();

                count.Should().Be(max);
                count2.Should().Be(max);

                var results = await Enumerable.Range(0, max)
                    .Select((x, i) => new Task<bool>[]
                    {
                        Task.Run(async () => await manager.Deactivate<ICache>(new ActorKey($"cache/test/{i}"))),
                        Task.Run(async () => await manager.Deactivate<ICache2>(new ActorKey($"cache/test/{i+100}"))),
                    })
                    .SelectMany(x => x)
                    .WhenAll();

                results.All(x => x == true).Should().BeTrue();

                count.Should().Be(0);
                count2.Should().Be(0);

                await manager.DeactivateAll();
                count.Should().Be(0);
                count2.Should().Be(0);
            }
        }

        private void CountControl(ref int subject, int value)
        {
            if (value > 0)
            {
                Interlocked.Increment(ref subject);
            }
            else
            {
                Interlocked.Decrement(ref subject);
            }
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

            public StringCache(Action<int> increment) => Increment = increment;

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

            public Task<bool> IsCached(string key) => Task.FromResult(_values.Contains(key));

            public Task Add(string key)
            {
                _values.Add(key);
                return Task.FromResult(0);
            }
        }

        private interface ICache2 : IActor
        {
            Task<bool> IsCached(string key);
            Task Add(string key);
            ActorKey GetActorKey();
            IActorManager GetActorManager();
        }

        private class StringCache2 : ActorBase, ICache2
        {
            private HashSet<string> _values = new HashSet<string>();

            public StringCache2(Action<int> increment) => Increment = increment;

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

            public Task<bool> IsCached(string key) => Task.FromResult(_values.Contains(key));

            public Task Add(string key)
            {
                _values.Add(key);
                return Task.FromResult(0);
            }
        }
    }
}
