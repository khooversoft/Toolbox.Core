using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Toolbox.Standard.Test.Extensions
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void GivenStack_WhenDrained_ShouldMatchList()
        {
            var list = new List<string>
            {
                "First",
                "Second",
                "Third",
            };

            var stack = list.Reverse<string>().ToStack();

            var drainedList = stack.Drain().ToList();

            stack.Count.Should().Be(0);
            list.Count.Should().Be(drainedList.Count);

            list
                .Zip(drainedList, (o, i) => (o, i))
                .All(x => x.i == x.o)
                .Should().BeTrue();
        }

        [Fact]
        public void GivenEmptyStack_WhenDrained_ShouldBeEmpty()
        {
            var list = new List<string>();
            var stack = list.ToStack();

            var drainedList = stack.Drain().ToList();

            stack.Count.Should().Be(0);
            list.Count.Should().Be(drainedList.Count);
        }
    }
}
