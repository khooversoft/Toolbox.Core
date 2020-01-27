using FluentAssertions;
using Khooversoft.MessageNet.Interface;
using System;
using Xunit;

namespace MessageNet.Interface.Test
{
    public class MessageUriBuilderTests
    {
        [Fact]
        public void GivenMessageUriBuilder_WhenCreated_ShouldBuild()
        {
            const string protocol = "protocol";
            const string networkId = "networkId";
            const string nodeId = "nodeId";

            var builder = new MessageUriBuilder()
                .SetProtocol(protocol)
                .SetNetworkId(networkId)
                .SetNodeId(nodeId);

            builder.Protocol.Should().Be(protocol);
            builder.NetworkId.Should().Be(networkId);
            builder.NodeId.Should().Be(nodeId);

            builder.Route.Should().NotBeNull();
            builder.Route.Count.Should().Be(0);

            var uri = builder.Build();

            uri.Protocol.Should().Be(builder.Protocol);
            uri.NetworkId.Should().Be(builder.NetworkId);
            uri.NodeId.Should().Be(builder.NodeId);

            uri.ToString().Should().NotBeNullOrWhiteSpace();
            uri.ToString().Should().Be("protocol://networkId/nodeId");
        }

        [Fact]
        public void GivenMessageUriBuilder_WhenCreatedWithRoute_ShouldBuild()
        {
            const string protocol = "protocol";
            const string networkId = "networkId";
            const string nodeId = "nodeId";
            const string route = "route1/route2";

            var builder = new MessageUriBuilder()
                .SetProtocol(protocol)
                .SetNetworkId(networkId)
                .SetNodeId(nodeId)
                .SetRoute(route);

            builder.Protocol.Should().Be(protocol);
            builder.NetworkId.Should().Be(networkId);
            builder.NodeId.Should().Be(nodeId);
            builder.Route.Build().ToString().Should().Be(route);

            builder.Route.Should().NotBeNull();
            builder.Route.Count.Should().Be(2);

            var uri = builder.Build();

            uri.Protocol.Should().Be(builder.Protocol);
            uri.NetworkId.Should().Be(builder.NetworkId);
            uri.NodeId.Should().Be(builder.NodeId);
            uri.Route.Should().Be(route);
            uri.ToString().Should().Be("protocol://networkId/nodeId/route1/route2");
        }

        [Theory]
        [InlineData("")]
        [InlineData("/ms://p/n")]
        [InlineData("ms")]
        [InlineData("ms:")]
        [InlineData("ms:/")]
        [InlineData("ms://")]
        [InlineData("ms://networkId")]
        [InlineData("ms://networkId/")]
        public void GivenUri_WhenParsesd_ShouldThrowException(string uri)
        {
            Action act = () => MessageUriBuilder.Parse(uri);

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("msgnet://network1/node1", "network1", "node1", "")]
        [InlineData("msgnet://network1/node1/route", "network1", "node1", "route")]
        [InlineData("msgnet://network1/node1/route1/route2", "network1", "node1", "route1/route2")]
        [InlineData("network1/node1/route1/route2", "network1", "node1", "route1/route2")]
        [InlineData("msgnet://networkId/node1/", "networkId", "node1", "")]
        public void GivenUri_WhenParsed_ShouldVerify(string uri, string networkId, string nodeId, string route)
        {
            var builder = MessageUriBuilder.Parse(uri);

            builder.Protocol.Should().Be("msgnet");
            builder.NetworkId.Should().Be(networkId);
            builder.NodeId.Should().Be(nodeId);
            builder.Route.Build().ToString().Should().Be(route);
        }
    }
}
