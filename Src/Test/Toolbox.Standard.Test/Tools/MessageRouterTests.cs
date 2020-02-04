using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class MessageRouterTests
    {
        [Fact]
        public async Task GivenSingleRoute_WhenMessageSent_ShouldReceive()
        {
            int count = 0;
            const int max = 10;

            var messageRouter = new MessageRouter<int>()
                .Add(x =>
                {
                    if (x % 2 == 0) count++;
                    return Task.FromResult(true);
                });

            await Enumerable.Range(0, max)
                .ForEachAsync(async x => await messageRouter.Post(x));

            count.Should().Be(max / 2);
        }

        [Fact]
        public async Task GivenTwoRoute_WhenMessageSent_ShouldReceive()
        {
            int evenCount = 0;
            int oddCount = 0;
            const int max = 10;

            var messageRouter = new MessageRouter<int>()
                .Add(x =>
                {
                    if (x % 2 == 0) Interlocked.Increment(ref evenCount);
                    return Task.FromResult(true);
                })
                .Add(x =>
                {
                    if (x % 2 != 0) Interlocked.Increment(ref oddCount);
                    return Task.FromResult(true);
                });

            await Enumerable.Range(0, max)
                .ForEachAsync(async x => await messageRouter.Post(x));

            evenCount.Should().Be(max / 2);
            oddCount.Should().Be(max / 2);
        }

        [Fact]
        public async Task GivenTwoRoute_WhenMessageAwait_ShouldReceive()
        {
            int evenCount = 0;
            int oddCount = 0;
            const int max = 1000;

            var messageRouter = new MessageRouter<int>()
                .Add(x =>
                {
                    if (x % 2 == 0) Interlocked.Increment(ref evenCount);
                    return Task.FromResult(true);
                })
                .Add(x =>
                {
                    if (x % 2 != 0) Interlocked.Increment(ref oddCount);
                    return Task.FromResult(true);
                });

            for(int i = 0; i < max; i++)
            {
                await messageRouter.Post(i);
            }

            evenCount.Should().Be(max / 2);
            oddCount.Should().Be(max / 2);
        }

        [Fact]
        public void GivenTwoRoute_WhenMessageSentOnDifferentTask_ShouldReceive()
        {
            int evenCount = 0;
            int oddCount = 0;
            const int max = 1000;

            var messageRouter = new MessageRouter<int>()
                .Add(x =>
                {
                    if (x % 2 == 0) Interlocked.Increment(ref evenCount);
                    return Task.FromResult(true);
                })
                .Add(x =>
                {
                    if (x % 2 != 0) Interlocked.Increment(ref oddCount);
                    return Task.FromResult(true);
                });

            var tasks = Enumerable.Range(0, max)
                .Select(x => Task.Run(() => messageRouter.Post(x)))
                .ToArray();

            Task.WaitAll(tasks);

            evenCount.Should().Be(max / 2);
            oddCount.Should().Be(max / 2);
        }
    }
}
