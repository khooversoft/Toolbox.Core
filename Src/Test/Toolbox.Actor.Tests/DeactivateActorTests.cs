// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khoover.Toolbox.TestTools;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
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
        private ILoggerFactory _loggerFactory = new TestLoggerFactory();

        [Fact]
        public async Task GivenActor_WhenDeactivated_ShouldPass()
        {
            ActorManager manager = new ActorManager(ActorConfigurationBuilder.Default, _loggerFactory);
            manager.Register<ICache>(() => new StringCache());

            ActorKey actorKey = new ActorKey("Cache/Test1");
            ICache cache = manager.GetActor<ICache>(actorKey);

            int count = await cache.GetCount();
            count.Should().Be(1);

            await cache.TestAndDeactivate();
            count = await cache.GetCount();
            count.Should().Be(2);

            await cache.TestAndDeactivate();
            count = await cache.GetCount();
            count.Should().Be(4);
        }

        private interface ICache : IActor
        {
            Task<int> GetCount();

            Task TestAndDeactivate();
        }

        private class StringCache : ActorBase, ICache
        {
            private int _count = 0;

            public StringCache()
            {
            }

            protected override Task OnActivate()
            {
                _count++;
                return base.OnActivate();
            }

            protected override Task OnDeactivate()
            {
                _count++;
                return base.OnDeactivate();
            }

            public Task<int> GetCount() => Task.FromResult(_count);

            public Task TestAndDeactivate()
            {
                if (_count++ >= 2) return Deactivate();

                return Task.CompletedTask;
            }
        }
    }
}
