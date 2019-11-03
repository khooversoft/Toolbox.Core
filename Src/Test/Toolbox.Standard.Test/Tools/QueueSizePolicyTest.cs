using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class QueueSizePolicyTest
    {
        [Fact]
        public void GivenQueuePolicy_WhenLessThenMaxAdded_NoDataLoss()
        {
            const int max = 10;
            const int count = 5;
            IQueueSizePolicy<int> queue = new Queue<int>().SetFixSizePolicy(max);

            Enumerable.Range(0, count).ForEach(x => queue.Enqueue(x));

            queue.Count.Should().Be(count);
            queue.LostCount.Should().Be(0);

            Enumerable.Range(0, count)
                .Zip(queue, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        [Fact]
        public void GivenQueuePolicy_WhenEqualMaxAdded_ShouldNoDataLoss()
        {
            const int max = 10;
            IQueueSizePolicy<int> queue = new Queue<int>().SetFixSizePolicy(max);

            Enumerable.Range(0, max).ForEach(x => queue.Enqueue(x));

            queue.Count.Should().Be(max);
            queue.LostCount.Should().Be(0);

            Enumerable.Range(0, max)
                .Zip(queue, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        [Fact]
        public void GivenQueuePolicy_WhenMoreThenMaxAdded_ShouldHaveDataLoss()
        {
            const int max = 10;
            const int count = 11;
            IQueueSizePolicy<int> queue = new Queue<int>().SetFixSizePolicy(max);

            Enumerable.Range(0, count).ForEach(x => queue.Enqueue(x));

            queue.Count.Should().Be(max);
            queue.LostCount.Should().Be(count-max);

            Enumerable.Range(1, max)
                .Zip(queue, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }
    }
}
