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
    public class GraphTopologicalDependsOnTests
    {
        [Fact]
        public void SimpleTopologicalTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphDependOnEdge<string>("Node2", "Node1"),
            };

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "Node1" },
                new List<string> { "Node2" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void SimpleMixTopologicalTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphEdge<string>("Node2", "Node1"),
                new GraphDependOnEdge<string>("Node2", "Node3"),
            };


            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "Node3" },
                new List<string> { "Node2" },
                new List<string> { "Node1" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void AllDependenciesTopologicalTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),
                new GraphDependOnEdge<string>("Node2", "Node1"),
                new GraphDependOnEdge<string>("Node3", "Node2"),
                new GraphDependOnEdge<string>("Node4", "Node3"),
            };


            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "Node1" },
                new List<string> { "Node2" },
                new List<string> { "Node3" },
                new List<string> { "Node4" },
            };

            Verify(sort, compare);
        }        

        [Fact]
        public void SimplePipelineTopological2Test()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("P0"),

                new GraphNode<string>("P0-A1"),
                new GraphEdge<string>("P0", "P0-A1"),

                new GraphNode<string>("P0-A2"),
                new GraphEdge<string>("P0", "P0-A2"),

                new GraphNode<string>("P0-A3"),
                new GraphDependOnEdge<string>("P0-A3", "P0-A2"),
            };

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "P0" },
                new List<string> { "P0-A1", "P0-A2" },
                new List<string> { "P0-A3" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void Level2PipelineTopological2Test()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("P0"),

                new GraphNode<string>("P0-A1"),
                new GraphEdge<string>("P0", "P0-A1"),

                new GraphNode<string>("P0-A2"),
                new GraphEdge<string>("P0", "P0-A2"),

                new GraphNode<string>("P0-A3"),
                new GraphEdge<string>("P0", "P0-A3"),
                new GraphDependOnEdge<string>("P0-A3", "P0-A1"),
                new GraphDependOnEdge<string>("P0-A3", "P0-A2"),

                new GraphNode<string>("P3-A1"),
                new GraphEdge<string>("P0-A3", "P3-A1"),

                new GraphNode<string>("P3-A2"),
                new GraphEdge<string>("P0-A3", "P3-A2"),
            };

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "P0" },
                new List<string> { "P0-A1", "P0-A2" },
                new List<string> { "P0-A3" },
                new List<string> { "P3-A1", "P3-A2" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void Level3PipelineTopological2Test()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("P0"),

                new GraphNode<string>("P0-A1"),
                new GraphEdge<string>("P0", "P0-A1"),

                new GraphNode<string>("P0-A2"),
                new GraphEdge<string>("P0", "P0-A2"),

                new GraphNode<string>("P0-A3"),
                new GraphEdge<string>("P0", "P0-A3"),
                new GraphDependOnEdge<string>("P0-A3", "P0-A2"),

                    new GraphNode<string>("P3-A1"),
                    new GraphEdge<string>("P0-A3", "P3-A1"),

                    new GraphNode<string>("P3-A2"),
                    new GraphEdge<string>("P0-A3", "P3-A2"),

                new GraphNode<string>("P0-A4"),
                new GraphDependOnEdge<string>("P0-A4", "P0-A1"),
                new GraphDependOnEdge<string>("P0-A4", "P0-A2"),
                new GraphDependOnEdge<string>("P0-A4", "P0-A3"),

                    new GraphNode<string>("P4-A1"),
                    new GraphEdge<string>("P0-A4", "P4-A1"),

                    new GraphNode<string>("P4-A2"),
                    new GraphEdge<string>("P0-A4", "P4-A2"),
            };

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "P0" },
                new List<string> { "P0-A1", "P0-A2" },
                new List<string> { "P0-A3" },
                new List<string> { "P3-A1", "P3-A2" },
                new List<string> { "P0-A4" },
                new List<string> { "P4-A1", "P4-A2" },
            };

            Verify(sort, compare);
        }

        private void Verify(IList<IList<IGraphNode<string>>> sort, IList<IList<string>> compare)
        {
            sort.Count.Should().Be(compare.Count);

            for (int i = 0; i < sort.Count; i++)
            {
                sort[i].Count.Should().Be(compare[i].Count);

                for (int j = 0; j < sort[i].Count; j++)
                {
                    sort[i][j].Key.Should().Be(compare[i][j]);
                }
            }
        }
    }
}
