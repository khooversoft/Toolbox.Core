using FluentAssertions;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Actor.Tests
{
    [Trait("Category", "Actor")]
    public class DeactivateActorTests
    {
        private IWorkContext _context = WorkContext.Empty;

        [Fact]
        public async Task GivenActor_WhenDeactivated_ShouldPass()
        {
            ActorManager manager = new ActorManager();
            manager.Register<ICache>(_ => new StringCache());

            ActorKey actorKey = new ActorKey("Cache/Test1");
            ICache cache = await manager.CreateProxy<ICache>(actorKey);

            int count = await cache.GetCount();
            count.Should().Be(1);

            await cache.TestAndDeactivate(_context);
            count = await cache.GetCount();
            count.Should().Be(2);

            await cache.TestAndDeactivate(_context);
            count = await cache.GetCount();
            count.Should().Be(4);
        }

        private interface ICache : IActor
        {
            Task<int> GetCount();

            Task TestAndDeactivate(IWorkContext context);
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

            public Task TestAndDeactivate(IWorkContext context)
            {
                if (_count++ >= 2) return Deactivate(context);

                return Task.CompletedTask;
            }
        }
    }
}
