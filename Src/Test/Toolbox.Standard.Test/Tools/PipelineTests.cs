using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class PipelineTests
    {
        [Fact]
        public void SimplePipelineClassTest()
        {
            var list = new List<int>();
            var list2 = new List<int>();

            var p = new PipelineManager<IWorkContext, int>
            {
                new Pipeline<IWorkContext, int>() + ((c, x) => { list.Add(x); return true; }),
                new Pipeline<IWorkContext, int>() + ((c, x) => { list2.Add(x + 1000); return true; }),
            };

            Enumerable.Range(0, 100)
                .ForEach(x => p.Post(null!, x));

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
        public void SimplePipelineClass2Test()
        {
            var list = new List<int>();
            var list2 = new List<int>();

            var p = new PipelineManager<IWorkContext, int>
            {
                new Pipeline<IWorkContext, int>
                {
                    ((c, x) => { list.Add(x); return true; }),
                    ((c, x) => { list2.Add(x + 1000); return true; }),
                }
            };

            Enumerable.Range(0, 100)
                .ForEach(x => p.Post(null, x));

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
        public void ActionQueueTestInPipeline()
        {
            var list = new List<List<int>>();

            var bb = new BatchBlock<int>(10);
            var ab = new ActionBlock<int[]>(x => list.Add(new List<int>(x)));
            bb.LinkTo(ab);

            var p = new PipelineManager<IWorkContext, int>
            {
                new Pipeline<IWorkContext, int>() + ((c, x) => { bb.Post(x); return true; }),
            };

            Enumerable.Range(0, 100)
                .ForEach(x => p.Post(null, x));

            bb.Complete();
            Task.WaitAll(bb.Completion);

            list.Count.Should().Be(10);
            list.All(x => x.Count == 10).Should().BeTrue();
        }
    }
}
