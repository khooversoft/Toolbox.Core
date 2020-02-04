﻿using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class DataflowTests
    {
        [Fact]
        public void GivenDataflow_WhenPostAsyncMessage_ShouldRoute()
        {
            const int max = 10;
            int count = 0;
            var list = new List<string>();

            IDataflowSource<string> dataflow = new DataflowBuilder<string>()
            {
                new ActionDataflow<string>(x => count++),
                new ActionDataflow<string>(x => list.Add(x))
            }.Build();

            Enumerable.Range(0, max)
                .ForEach(async x => await dataflow.PostAsync($"{x} message"));

            dataflow.Complete();
            dataflow.Completion.Wait();

            count.Should().Be(max);
            list.Count.Should().Be(max);
        }

        [Fact]
        public void GivenPipelineBlock_WhenPostMessage_ShouldReceiveMessage()
        {
            const int max = 10;
            int count = 0;
            var list = new List<string>();

            IDataflowSource<string> dataflow = new DataflowBuilder<string>()
            {
                new ActionDataflow<string>(x => count++),
                new ActionDataflow<string>(x => list.Add(x))
            }.Build();

            Enumerable.Range(0, max)
                .ForEach(x => dataflow.Post($"{x} message"));

            dataflow.Complete();
            dataflow.Completion.Wait();

            count.Should().Be(max);
            list.Count.Should().Be(max);
        }

        [Fact]
        public void GivenDataflow_WhenSendMessage_ShouldReceiveMessage()
        {
            const int max = 100;
            var list = new List<int>();
            var list2 = new List<int>();

            IDataflowSource<int> dataflow = new DataflowBuilder<int>()
            {
                new ActionDataflow<int>(x => list.Add(x)),
                new ActionDataflow<int>(x => list2.Add(x + 1000))
            }.Build();

            Enumerable.Range(0, max)
                .ForEach(x => dataflow.Post(x).Verify().Assert(x => x == true, "Failed to post"));

            dataflow.Complete();
            dataflow.Completion.Wait();

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
        public void GivenDataflowBlock_ApplyTransform_ShouldPass()
        {
            const int max = 1000;
            const int offset = 10000;
            var list = new List<int>();
            var list2 = new List<int>();

            IDataflowSource<int> dataflow = new DataflowBuilder<int>()
            {
                new SelectDataflow<int>(x => x + 5),
                new BroadcastDataflow<int>()
                {
                    new ActionDataflow<int>(x => list.Add(x)),
                    new ActionDataflow<int>(x => list2.Add(x + offset))
                }
            }.Build();

            Enumerable.Range(0, max)
                .ForEach(async x => await dataflow.PostAsync(x));

            dataflow.Complete();
            dataflow.Completion.Wait();

            list.Count.Should().Be(max);
            list2.Count.Should().Be(max);

            list
                .Select((x, i) => new { x, i })
            .All(x => x.x == x.i + 5)
            .Should().BeTrue();

            list2
                .Select((x, i) => new { i, x })
            .All(x => x.i + offset + 5 == x.x)
            .Should().BeTrue();
        }


        [Fact]
        public void GivenBuffer_ApplyTransformAndTwoSubpiplines_ShouldPass()
        {
            const int max = 1000;
            const int offset = 10000;
            var list = new List<int>();
            var list2 = new List<int>();
            var s1 = new List<int>();
            var s2 = new List<int>();

            IDataflowSource<int> dataflow = new DataflowBuilder<int>()
            {
                new SelectDataflow<int>(x => x + 5),
                new BroadcastDataflow<int>()
                {
                    new ActionDataflow<int>(x => list.Add(x)),
                    new ActionDataflow<int>(x => list2.Add(x + offset)),

                    new BroadcastDataflow<int>()
                    {
                        new ActionDataflow<int>(x => s1.Add(x))
                    },

                    new BroadcastDataflow<int>()
                    {
                        new ActionDataflow<int>(x => s2.Add(x))
                    },
                }
            }.Build();

            Enumerable.Range(0, max)
                .ForEach(async x => await dataflow.PostAsync(x));

            dataflow.Complete();
            dataflow.Completion.Wait();

            list.Count.Should().Be(max);
            list2.Count.Should().Be(max);
            s1.Count.Should().Be(max);
            s2.Count.Should().Be(max);

            list
                .Select((x, i) => new { x, i })
                .All(x => x.x == x.i + 5)
                .Should().BeTrue();

            list2
                .Select((x, i) => new { i, x })
                .All(x => x.i + offset + 5 == x.x)
                .Should().BeTrue();

            s1
                .Select((x, i) => new { x, i })
                .All(x => x.x == x.i + 5)
                .Should().BeTrue();

            s2
                .Select((x, i) => new { x, i })
                .All(x => x.x == x.i + 5)
                .Should().BeTrue();
        }
    }
}
