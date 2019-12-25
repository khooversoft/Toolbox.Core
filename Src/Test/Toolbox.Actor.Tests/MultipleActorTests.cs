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

namespace Toolbox.Actor.Tests
{
    public class MultipleActorTests
    {
        [Fact]
        public async Task Given2Actors_WhenCreatedAndDeleted_ShouldPass()
        {
            int count = 0;
            int count2 = 0;
            const int max = 10;

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => count += y));
            manager.Register<ICache2>(_ => new StringCache2(y => count2 += y));

            await Enumerable.Range(0, max)
                .Select(async x =>
                {
                    ActorKey key = new ActorKey($"cache/test/{x}");
                    ICache cache = await manager.CreateProxy<ICache>(key);
                    cache.GetActorKey().Should().Be(key);
                    cache.GetActorManager().Should().Be(manager);

                    ActorKey key2 = new ActorKey($"cache/test/{x}");
                    ICache2 cache2 = await manager.CreateProxy<ICache2>(key2);
                    cache2.GetActorKey().Should().Be(key2);
                    cache2.GetActorManager().Should().Be(manager);
                })
                .WhenAll();

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

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => count += y));
            manager.Register<ICache2>(_ => new StringCache2(y => count2 += y));

            await Enumerable.Range(0, max)
                .Select((x, i) => new Task[]
                {
                    Task.Run(async () => {
                        ActorKey key = new ActorKey($"cache/test/{i}");
                        ICache cache = await manager.CreateProxy<ICache>(key);
                        cache.GetActorKey().Should().Be(key);
                        cache.GetActorManager().Should().Be(manager);
                    }),
                    Task.Run(async () => {
                        ActorKey key2 = new ActorKey($"cache/test/{i}");
                        ICache2 cache2 = await manager.CreateProxy<ICache2>(key2);
                        cache2.GetActorKey().Should().Be(key2);
                        cache2.GetActorManager().Should().Be(manager);
                    }),
                })
                .SelectMany(x => x)
                .WhenAll();

            count.Should().Be(max);
            count2.Should().Be(max);

            await Enumerable.Range(0, max)
                .Select((x, i) => new Task[]
                {
                    Task.Run(async () => {
                        ActorKey key = new ActorKey($"cache/test/{i}");
                        (await manager.Deactivate<ICache>(key)).Should().BeTrue();
                    }),
                    Task.Run(async () => {
                        ActorKey key2 = new ActorKey($"cache/test/{i}");
                        (await manager.Deactivate<ICache2>(key2)).Should().BeTrue();
                    }),
                })
                .SelectMany(x => x)
                .WhenAll();

            count.Should().Be(0);
            count2.Should().Be(0);
            
            await manager.DeactivateAll();
            count.Should().Be(0);
            count2.Should().Be(0);
        }

        [Fact]
        public async Task Given2Actors_WhenCreatedAndDeletedDifferentKeyRange_ShouldPass()
        {
            int count = 0;
            int count2 = 0;
            const int max = 10;

            IActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache(y => count += y));
            manager.Register<ICache2>(_ => new StringCache2(y => count2 += y));

            await Enumerable.Range(0, max)
                .Select((x, i) => new Task[]
                {
                    Task.Run(() => {
                        ActorKey key = new ActorKey($"cache/test/{i}");
                        return manager.CreateProxy<ICache>(key);
                    }),
                    Task.Run(() => {
                        ActorKey key2 = new ActorKey($"cache/test/{i+100}");
                        return manager.CreateProxy<ICache2>(key2);
                    }),
                })
                .SelectMany(x => x)
                .WhenAll();

            count.Should().Be(max);
            count2.Should().Be(max);

            await Enumerable.Range(0, max)
                .Select((x, i) => new Task[]
                {
                    Task.Run(async () => {
                        ActorKey key = new ActorKey($"cache/test/{i}");
                        (await manager.Deactivate<ICache>(key)).Should().BeTrue();
                    }),
                    Task.Run(async () => {
                        ActorKey key2 = new ActorKey($"cache/test/{i+100}");
                        (await manager.Deactivate<ICache2>(key2)).Should().BeTrue();
                    }),
                })
                .SelectMany(x => x)
                .WhenAll();

            count.Should().Be(0);
            count2.Should().Be(0);

            await manager.DeactivateAll();
            count.Should().Be(0);
            count2.Should().Be(0);
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
