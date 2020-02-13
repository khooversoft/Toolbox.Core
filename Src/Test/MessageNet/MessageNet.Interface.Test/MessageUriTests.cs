// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.MessageNet.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MessageNet.Interface.Test
{
    public class MessageUriTests
    {
        [Fact]
        public void GvenCorrectParameter_WhenMessageUriConstructed_ShouldPass()
        {
            var uri = new MessageUri("protocol", "networkId", "nodeId");

            uri.Protocol.Should().Be("protocol");
            uri.NetworkId.Should().Be("networkId");
            uri.NodeId.Should().Be("nodeId");
        }

        [Fact]
        public void GvenCorrectAndRouteParameter_WhenMessageUriConstructed_ShouldPass()
        {
            var uri = new MessageUri("protocol", "networkId", "nodeId", "route1/route2");

            uri.Protocol.Should().Be("protocol");
            uri.NetworkId.Should().Be("networkId");
            uri.NodeId.Should().Be("nodeId");
            uri.Route.Should().Be("route1/route2");

            uri.ToString().Should().Be("protocol://networkId/nodeId/route1/route2");
        }

        [Theory]
        [InlineData(null, "networkId", "nodeId")]
        [InlineData("protocol", null, "nodeId")]
        [InlineData("protocol", "networkId", null)]
        [InlineData("protocol/", "networkId", "nodeId")]
        [InlineData("protocol", "netw//orkId", "nodeId")]
        [InlineData("protocol", "networkId", "no/de/Id")]
        [InlineData("proto&col", "networkId", "nodeId")]
        [InlineData("protocol", "netw*orkId", "nodeId")]
        [InlineData("protocol", "networkId", "nod(eId")]
        public void GvenMissingParameter_WhenMessageUriConstructed_ShouldThrowException(string protocol, string networkId, string nodeId)
        {
            Action act = () => new MessageUri(protocol, networkId, nodeId);

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("protocol.1", "networkId-2", "nodeId-3.first")]
        [InlineData("protocol1234", "NETWORKID-2", "NODEID-3.FIRST")]
        public void GvenValidParameter_WhenMessageUriConstructed_ShouldNotThrowException(string protocol, string networkId, string nodeId)
        {
            Action act = () => new MessageUri(protocol, networkId, nodeId);

            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("", "protocol://networkId/nodeId")]
        [InlineData("route1", "protocol://networkId/nodeId/route1")]
        [InlineData("route1/route2/route3", "protocol://networkId/nodeId/route1/route2/route3")]
        public void GvenRouteParameter_WhenMessageUriConstructed_ShouldThrowException(string route, string expectedUri)
        {
            var uri = new MessageUri("protocol", "networkId", "nodeId", route);

            uri.Protocol.Should().Be("protocol");
            uri.NetworkId.Should().Be("networkId");
            uri.NodeId.Should().Be("nodeId");
            uri.Route.Should().Be(route);
            uri.ToString().Should().Be(expectedUri);
        }

    }
}
