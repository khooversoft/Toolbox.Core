using FluentAssertions;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using Xunit;

namespace Toolbox.Graph.Test
{
    public class GraphLeafTests
    {
        [Fact]
        public void NoNodeTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
            };

            map.Invoking(x => x.GetLeafNodes(null)).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void OneNodeTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
            };

            IReadOnlyList<IGraphNode<string>> nodes = map.GetLeafNodes(map.Nodes["Node1"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(0);
        }

        [Fact]
        public void TwoNodesTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
            };

            IReadOnlyList<IGraphNode<string>> nodes = map.GetLeafNodes(map.Nodes["Node1"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(0);

            nodes = map.GetLeafNodes(map.Nodes["Node2"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(0);
        }

        [Fact]
        public void TwoNodesWithEdgeTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphEdge<string>("Node1", "Node2"),
            };

            IReadOnlyList<IGraphNode<string>> nodes = map.GetLeafNodes(map.Nodes["Node1"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(1);
            nodes[0].Key.Should().Be("Node2");

            nodes = map.GetLeafNodes(map.Nodes["Node2"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(0);
        }

        [Fact]
        public void TwoNodesWithEdgeReverseTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphEdge<string>("Node2", "Node1"),
            };

            IReadOnlyList<IGraphNode<string>> nodes = map.GetLeafNodes(map.Nodes["Node2"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(1);
            nodes[0].Key.Should().Be("Node1");

            nodes = map.GetLeafNodes(map.Nodes["Node1"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(0);
        }

        [Fact]
        public void EdgeCircularTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>(strict:false)
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphEdge<string>("Node1", "Node2"),
                new GraphEdge<string>("Node2", "Node1"),
            };

            IReadOnlyList<IGraphNode<string>> nodes = map.GetLeafNodes(map.Nodes["Node1"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(0);

            nodes = map.GetLeafNodes(map.Nodes["Node2"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(0);
        }

        [Fact]
        public void ComplexGraphTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),

                new GraphEdge<string>("Node1", "Node2"),
                new GraphEdge<string>("Node2", "Node3"),
                new GraphEdge<string>("Node1", "Node4"),
            };

            IReadOnlyList<IGraphNode<string>> nodes = map.GetLeafNodes(map.Nodes["Node1"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(2);
            nodes[0].Key.Should().Be("Node4");
            nodes[1].Key.Should().Be("Node3");

            nodes = map.GetLeafNodes(map.Nodes["Node2"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(1);
            nodes[0].Key.Should().Be("Node3");

            nodes = map.GetLeafNodes(map.Nodes["Node3"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(0);

            nodes = map.GetLeafNodes(map.Nodes["Node4"]);
            nodes.Should().NotBeNull();
            nodes.Count.Should().Be(0);
        }
    }
}
