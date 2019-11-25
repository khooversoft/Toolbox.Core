﻿using Autofac;
using FluentAssertions;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Actor
{
    [Trait("Category", "Actor")]
    public class ActorTimerTests
    {
        private IWorkContext _context = WorkContext.Empty;

        [Fact]
        public async Task ActorSimpleTimerTest()
        {
            IActorManager manager = new ActorManager();
            manager.Register<ITimerActor>(_context, _ => new TimeActor());

            ActorKey key = new ActorKey("timer/test");
            ITimerActor timerActor = await manager.CreateProxy<ITimerActor>(_context, key);

            foreach (var index in Enumerable.Range(0, 20))
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                int count = await timerActor.GetCount();
                if (count > 2) break;
            }

            (await timerActor.GetCount()).Should().BeGreaterThan(2);
            await manager.Deactivate<ITimerActor>(_context, key);
        }

        private interface ITimerActor : IActor
        {
            Task<int> GetCount();
        }

        private class TimeActor : ActorBase, ITimerActor
        {
            private HashSet<string> _values = new HashSet<string>();
            private int _count = 0;

            public TimeActor()
            {
            }

            protected override Task OnActivate(IWorkContext context)
            {
                SetTimer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

                return base.OnActivate(context);
            }

            protected override Task OnTimer()
            {
                _count++;
                return Task.FromResult(0);
            }

            public Task<int> GetCount()
            {
                return Task.FromResult(_count);
            }
        }
    }
}