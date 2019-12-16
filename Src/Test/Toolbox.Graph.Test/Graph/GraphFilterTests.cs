// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Graph.Test
{
    public class GraphFilterTests
    {
        [Fact]
        public void SimpleNoChildrenFilterGraphTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),
            };

            var filter = map.CreateFilter()
                .Include(map.Nodes["Node1"].Key);

            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> newMap = map.Create(filter);
            newMap.Nodes.Count.Should().Be(1);
            newMap.Nodes.Values.Any(x => x.Key == "Node1").Should().BeTrue();
        }

        [Fact]
        public void AllFilterGraphTests()
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

            var filter = map.CreateFilter()
                .Include(map.Nodes["Node1"].Key);

            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> newMap = map.Create(filter);

            newMap.Nodes.Count.Should().Be(4);
            newMap.Nodes.Values.Any(x => x.Key == "Node1").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node2").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node3").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node4").Should().BeTrue();
        }

        [Fact]
        public void SubNodeFilterGraphTests()
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

            var filter = map.CreateFilter()
                .Include(map.Nodes["Node2"].Key);

            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> newMap = map.Create(filter);
            newMap.Nodes.Count.Should().Be(2);
            newMap.Nodes.Values.Any(x => x.Key == "Node2").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node3").Should().BeTrue();
        }

        [Fact]
        public void OneFilterGraphTests()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),

                new GraphEdge<string>("Node1", "Node2"),
                new GraphEdge<string>("Node3", "Node4"),
            };

            var filter = map.CreateFilter()
                .Include(map.Nodes["Node1"].Key);

            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> newMap = map.Create(filter);
            newMap.Nodes.Count.Should().Be(2);
            newMap.Nodes.Values.Any(x => x.Key == "Node1").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node2").Should().BeTrue();
        }

        [Fact]
        public void SubReverseNodeFilterGraphTests()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),

                new GraphEdge<string>("Node2", "Node1"),
                new GraphEdge<string>("Node1", "Node4"),
            };

            var filter = map.CreateFilter()
                .Include(map.Nodes["Node2"].Key);

            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> newMap = map.Create(filter);
            newMap.Nodes.Count.Should().Be(3);
            newMap.Nodes.Values.Any(x => x.Key == "Node2").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node1").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node4").Should().BeTrue();
        }

        [Fact]
        public void MultipleSameNodesFilterGraphTests()
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

            var filter = map.CreateFilter()
                .Include(map.Nodes["Node1"].Key, map.Nodes["Node1"].Key);

            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> newMap = map.Create(filter);

            newMap.Nodes.Count.Should().Be(4);
            newMap.Nodes.Values.Any(x => x.Key == "Node1").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node2").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node3").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node4").Should().BeTrue();
        }

        [Fact]
        public void MultipleNodesFilterGraphTests()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),
                new GraphNode<string>("Node5"),

                new GraphEdge<string>("Node1", "Node2"),
                new GraphEdge<string>("Node2", "Node3"),
                new GraphEdge<string>("Node1", "Node4"),
                new GraphEdge<string>("Node4", "Node5"),
            };

            var filter = map.CreateFilter()
                .Include(map.Nodes["Node2"].Key, map.Nodes["Node4"].Key);

            GraphMap<string, IGraphNode<string>, IGraphEdge<string>> newMap = map.Create(filter);

            newMap.Nodes.Count.Should().Be(4);
            newMap.Nodes.Values.Any(x => x.Key == "Node2").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node4").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node3").Should().BeTrue();
            newMap.Nodes.Values.Any(x => x.Key == "Node5").Should().BeTrue();
        }
    }
}
