// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using Khooversoft.Toolbox.Actor;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Khoover.Toolbox.TestTools;

namespace Toolbox.Actor.Tests
{
    [Trait("Category", "Actor")]
    public class ActorChainCallTests
    {
        private const string sumActorName = "actorSum";
        private ILoggerFactory _loggerFactory = new TestLoggerFactory();

        [Fact]
        public async Task ActorSingleChainTest()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ActorNode>().As<IActorNode>();
            builder.RegisterType<ActorSum>().As<IActorSum>();
            ILifetimeScope container = builder.Build();

            IActorManager manager = new ActorConfigurationBuilder()
                .Build()
                .ToActorManager(_loggerFactory)
                .Register(() => container.Resolve<IActorNode>())
                .Register(() => container.Resolve<IActorSum>());

            using (container)
            {
                ActorKey key = new ActorKey("node/test");
                IActorNode node = manager.GetActor<IActorNode>(key);

                int sum = 0;
                for (int i = 0; i < 10; i++)
                {
                    await node.Add(i);
                    sum += i;
                }

                IActorSum sumActor = manager.GetActor<IActorSum>(new ActorKey(sumActorName));
                (await sumActor.GetSum()).Should().Be(sum);
            }

            await manager.DeactivateAll();
        }

        private interface IActorNode : IActor
        {
            Task Add(int value);
        }

        private interface IActorSum : IActor
        {
            Task Add(int value);

            Task<int> GetSum();
        }

        private class ActorNode : ActorBase, IActorNode
        {
            private IActorSum? _actorSum;

            public ActorNode()
            {
            }

            public async Task Add(int value)
            {
                _actorSum = _actorSum ?? ActorManager.GetActor<IActorSum>(new ActorKey(sumActorName));
                await _actorSum.Add(value);
            }
        }

        private class ActorSum : ActorBase, IActorSum
        {
            private int _sum;

            public ActorSum()
            {
            }

            public Task Add(int value)
            {
                _sum += value;
                return Task.FromResult(0);
            }

            public Task<int> GetSum()
            {
                return Task.FromResult(_sum);
            }
        }
    }
}
