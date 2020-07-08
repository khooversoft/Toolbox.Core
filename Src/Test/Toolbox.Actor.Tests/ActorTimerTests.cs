// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khoover.Toolbox.TestTools;
using Khooversoft.Toolbox.Actor;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Actor.Tests
{
    [Trait("Category", "Actor")]
    public class ActorTimerTests
    {
        private ILoggerFactory _loggerFactory = new TestLoggerFactory();

        [Fact]
        public async Task ActorSimpleTimerTest()
        {
            IActorManager manager = new ActorManager(ActorConfigurationBuilder.Default, _loggerFactory);
            manager.Register<ITimerActor>(() => new TimeActor());

            ActorKey key = new ActorKey("timer/test");
            ITimerActor timerActor = manager.GetActor<ITimerActor>(key);

            foreach (var index in Enumerable.Range(0, 20))
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                int count = await timerActor.GetCount();
                if (count > 2) break;
            }

            (await timerActor.GetCount()).Should().BeGreaterThan(2);
            await manager.Deactivate<ITimerActor>(key);
        }

        private interface ITimerActor : IActor
        {
            Task<int> GetCount();
        }

        private class TimeActor : ActorBase, ITimerActor
        {
            private int _count = 0;

            public TimeActor()
            {
            }

            protected override Task OnActivate()
            {
                SetTimer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

                return base.OnActivate();
            }

            protected override Task OnTimer()
            {
                _count++;
                return Task.FromResult(0);
            }

            public Task<int> GetCount() => Task.FromResult(_count);
        }
    }
}