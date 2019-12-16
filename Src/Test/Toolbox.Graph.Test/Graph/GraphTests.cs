// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using KHooversoft.Toolbox.Graph;
using System;
using Xunit;

namespace Toolbox.Graph.Test
{
    public class GraphTests
    {
        [Fact]
        public void EmptyNodeTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>();
        }

        [Fact]
        public void OneNodeTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
            };

            map.Nodes.Count.Should().Be(1);
            map.Edges.Count.Should().Be(0);
        }

        [Fact]
        public void TwoNodesErrorTest()
        {
            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> map = null;

            Action test = () =>
            {
                map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
                {
                    new GraphNode<string>("Node1"),
                    new GraphNode<string>("Node1"),
                };
            };

            test.Should().Throw<ArgumentException>();
            map.Should().BeNull();
        }

        [Fact]
        public void TwoNodesTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
            };

            map.Nodes.Count.Should().Be(2);
            map.Edges.Count.Should().Be(0);
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

            map.Nodes.Count.Should().Be(2);
            map.Edges.Count.Should().Be(1);
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

            map.Nodes.Count.Should().Be(2);
            map.Edges.Count.Should().Be(1);
        }

        [Fact]
        public void TwoNodesWithEdgeErrorsTest()
        {
            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> map = null;

            Action test = () =>
            {
                map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
                {
                    new GraphNode<string>("Node1"),
                    new GraphNode<string>("Node2"),
                    new GraphEdge<string>("Node2", "Node-fake"),
                };
            };

            test.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void EdgeSameNodeTest()
        {
            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> map = null;

            Action test = () =>
            {
                map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
                {
                    new GraphNode<string>("Node1"),
                    new GraphNode<string>("Node2"),
                    new GraphEdge<string>("Node1", "Node1"),
                };
            };

            test.Should().Throw<ArgumentException>();
            map.Should().BeNull();
        }

        [Fact]
        public void EdgeCircularFailTest()
        {
            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> map = null;

            Action test = () =>
            {
                map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
                {
                    new GraphNode<string>("Node1"),
                    new GraphNode<string>("Node2"),
                    new GraphEdge<string>("Node1", "Node2"),
                    new GraphEdge<string>("Node2", "Node1"),
                };
            };

            test.Should().Throw<ArgumentException>();
            map.Should().BeNull();
        }
    }
}
