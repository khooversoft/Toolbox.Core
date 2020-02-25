using FluentAssertions;
using Khooversoft.MessageNet.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MessageNet.Interface.Test
{
    public class RouteExtensionsTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "nodeId")]
        [InlineData("networkId", null )]
        [InlineData("net-workId", "nodeId")]
        [InlineData("networkId", "node-Id")]
        public void GivenRouteRequest_WhenEmpty_ShouldNotIsValid(string networkId, string nodeId)
        {
            var routeRequest = new RouteRequest
            {
                NetworkId = networkId,
                NodeId = nodeId
            };

            routeRequest.IsValid().Should().BeFalse();
        }

        [Theory]
        [InlineData("networkId", "nodeId" )]
        [InlineData("n123", "n383" )]
        [InlineData("n123", "n3.83" )]
        public void GivenRouteRequest_WhenEmpty_ShouldIsValid(string networkId, string nodeId)
        {
            var routeRequest = new RouteRequest
            {
                NetworkId = networkId,
                NodeId = nodeId
            };

            routeRequest.IsValid().Should().BeTrue();
        }
    }
}
