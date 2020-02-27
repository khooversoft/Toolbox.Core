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
            var uri = new MessageUri("protocol", "namespace", "networkId", "nodeId");

            uri.Protocol.Should().Be("protocol");
            uri.Namespace.Should().Be("namespace");
            uri.NetworkId.Should().Be("networkId");
            uri.NodeId.Should().Be("nodeId");
        }

        [Fact]
        public void GvenCorrectAndRouteParameter_WhenMessageUriConstructed_ShouldPass()
        {
            var uri = new MessageUri("protocol", "namespace", "networkId", "nodeId", "route1/route2");

            uri.Protocol.Should().Be("protocol");
            uri.NetworkId.Should().Be("networkId");
            uri.NodeId.Should().Be("nodeId");
            uri.Route.Should().Be("route1/route2");

            uri.ToString().Should().Be("protocol://namespace/networkId/nodeId/route1/route2");
        }

        [Theory]
        [InlineData("protocol", null, null, null)]
        [InlineData("protocol/", "namespace", "networkId", "nodeId")]
        [InlineData("protocol", "names-pace", "networkId", "nodeId")]
        [InlineData("protocol", "namespace", "netw//orkId", "nodeId")]
        [InlineData("protocol", "namespace", "networkId", "no/de/Id")]
        [InlineData("proto&col", "namespace", "networkId", "nodeId")]
        [InlineData("protocol", "namespace", "netw*orkId", "nodeId")]
        [InlineData("protocol", "namespace", "networkId", "nod(eId")]
        [InlineData("protocol.1", "namespace", "networkId-2", "nodeId-3.first")]
        [InlineData("protocol1234", "namespace", "NETWORKID-2", "NODEID-3.FIRST")]
        public void GvenMissingParameter_WhenMessageUriConstructed_ShouldThrowException(string protocol, string nameSpace, string networkId, string nodeId)
        {
            Action act = () => new MessageUri(protocol, nameSpace, networkId, nodeId);

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("", "protocol://default/networkId/nodeId")]
        [InlineData("route1", "protocol://default/networkId/nodeId/route1")]
        [InlineData("route1/route2/route3", "protocol://default/networkId/nodeId/route1/route2/route3")]
        public void GvenRouteParameter_WhenMessageUriConstructed_ShouldThrowException(string route, string expectedUri)
        {
            var uri = new MessageUri("protocol", "default", "networkId", "nodeId", route);

            uri.Protocol.Should().Be("protocol");
            uri.NetworkId.Should().Be("networkId");
            uri.NodeId.Should().Be("nodeId");
            uri.Route.Should().Be(route);
            uri.ToString().Should().Be(expectedUri);
        }

    }
}
