using FluentAssertions;
using Khooversoft.MessageNet.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MessageNet.Interface.Test
{
    public class QueueIdTests
    {
        [Theory]
        [InlineData("a")]
        [InlineData("abc")]
        [InlineData("a123")]
        public void GivenNetworkId_WhenQueueIdIsCreated_ShouldMatch(string id)
        {
            Action act = () => new QueueId(id, "nodeId");
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("134")]
        [InlineData("1.34")]
        [InlineData("a1.23")]
        [InlineData("a1/23")]
        public void GivenNetworkId_WhenQueueIdIsCreated_ShouldNotMatch(string id)
        {
            Action act = () => new QueueId(id, "nodeId");
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("abc")]
        [InlineData("a123")]
        [InlineData("a123.abce")]
        [InlineData("a123.ABCD.this")]
        public void GivenNodeId_WhenQueueIdIsCreated_ShouldMatch(string id)
        {
            Action act = () => new QueueId("networkId", id);
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("134")]
        [InlineData("a1/23")]
        [InlineData("a1-23")]
        [InlineData("-a123")]
        [InlineData("+a123")]
        public void GivenNodekId_WhenQueueIdIsCreated_ShouldNotMatch(string id)
        {
            Action act = () => new QueueId("networkId", id);
            act.Should().Throw<ArgumentException>();
        }
    }
}
