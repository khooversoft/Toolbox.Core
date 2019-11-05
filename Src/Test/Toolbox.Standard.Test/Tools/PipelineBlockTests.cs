using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class PipelineBlockTests
    {
        [Fact]
        public void GivenFrameworkBroadcastBlock_WhenAsyncSendMessage_ShouldReceiveMessage()
        {
            const int max = 10;
            int count = 0;
            var list = new List<string>();
            var readList = new List<string>();

            var broadcast = new BroadcastBlock<string>(x => x);
            var action1 = new ActionBlock<string>(x => { count++; readList.Add(x); });
            broadcast.LinkTo(action1, new DataflowLinkOptions { PropagateCompletion = true });

            var action2 = new ActionBlock<string>(x => list.Add(x));
            broadcast.LinkTo(action2, new DataflowLinkOptions { PropagateCompletion = true });

            Enumerable.Range(0, max)
                .ForEach(async x => await broadcast.SendAsync($"{x} message"));

            broadcast.Complete();
            Task.WaitAll(broadcast.Completion, action1.Completion, action2.Completion);

            count.Should().Be(max);
            list.Count.Should().Be(max);
        }

        [Fact]
        public void GivenBroadcastBlock_WhenAsyncSendMessage_ShouldReceiveMessage()
        {
            const int max = 10;
            int count = 0;
            var list = new List<string>();

            var pipeline = new PipelineBlock<string>()
                .Broadcast()
                .RouteTo(x => count++)
                .RouteTo(x => list.Add(x));

            Enumerable.Range(0, max)
                .ForEach(async x => await pipeline.SendAsync($"{x} message"));

            pipeline.Complete();
            pipeline.Completion.Wait();

            count.Should().Be(max);
            list.Count.Should().Be(max);
        }

        [Fact]
        public void GivenBroadcastBlock_WhenPostMessage_ShouldReceiveMessage()
        {
            const int max = 10;
            int count = 0;
            var list = new List<string>();

            var pipeline = new PipelineBlock<string>()
                .Broadcast()
                .RouteTo(x => count++)
                .RouteTo(x => list.Add(x));

            Enumerable.Range(0, max)
                .ForEach(x => pipeline.Post($"{x} message"));

            pipeline.Complete();
            pipeline.Completion.Wait();

            count.Should().Be(max);
            list.Count.Should().Be(max);
        }

        [Fact]
        public void GivenBroadcastBlock_WhenSendMessage_ShouldReceiveMessage()
        {
            const int max = 100;
            var list = new List<int>();
            var list2 = new List<int>();

            var jobs = new PipelineBlock<int>()
                .Broadcast()
                .RouteTo(x => list.Add(x))
                .RouteTo(x => list2.Add(x + 1000));

            Enumerable.Range(0, max)
                .ForEach(x => jobs.Post(x).Verify().Assert(x => x == true, "Failed to post"));

            jobs.Complete();
            jobs.Completion.Wait();

            list.Count.Should().Be(max);
            list2.Count.Should().Be(max);

            list
                .Select((i, x) => new { i, x })
                .All(x => x.i == x.x)
                .Should().BeTrue();

            list2
                .Select((x, i) => new { i, x })
                .All(x => x.i + 1000 == x.x)
                .Should().BeTrue();
        }

        [Fact]
        public void GivenBuffer_ApplyTransform_ShouldPass()
        {
            const int max = 100;
            var list = new List<int>();
            var list2 = new List<int>();

            var jobs = new PipelineBlock<int>()
                .Select(x => x + 5)
                .Broadcast()
                .RouteTo(x => list.Add(x))
                .RouteTo(x => list2.Add(x + 1000));

            Enumerable.Range(0, max)
                .ForEach(async x => await jobs.SendAsync(x));

            jobs.Complete();
            jobs.Completion.Wait();

            list.Count.Should().Be(max);
            list2.Count.Should().Be(max);

            var x1 = list
                .Select((x, i) => new { x, i })
            .All(x => x.x == x.i + 5)
            .Should().BeTrue();

            var x = list2
                .Select((x, i) => new { i, x })
            .All(x => x.i + 1000 + 5 == x.x)
            .Should().BeTrue();
        }
    }
}
