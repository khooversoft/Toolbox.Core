using FluentAssertions;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.Graph.Test
{
    public class GraphTopologicalContextTests
    {
        [Fact]
        public void SimpleTopologicalContextTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphEdge<string>("Node1", "Node2"),
            };

            var graphContext = new GraphTopologicalContext<String, IGraphEdge<string>>(maxLevels: 1);

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort(graphContext);

            var compare = new List<IList<string>>
            {
                new List<string> { "Node1" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void SimpleTopological2ContextTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphEdge<string>("Node1", "Node2"),
            };

            var graphContext = new GraphTopologicalContext<String, IGraphEdge<string>>(maxLevels: 1)
                .AddProcessedNodeKey("Node1");

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort(graphContext);

            var compare = new List<IList<string>>
            {
                new List<string> { "Node2" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void SimpleTopological3ContextTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphEdge<string>("Node1", "Node2"),
            };

            var graphContext = new GraphTopologicalContext<String, IGraphEdge<string>>(maxLevels: 1)
                .AddStopNodeKey("Node1");

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort(graphContext);

            var compare = new List<IList<string>>();

            Verify(sort, compare);
        }

        [Fact]
        public void SimpleTopological4ContextTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphEdge<string>("Node1", "Node2"),
            };

            var graphContext = new GraphTopologicalContext<String, IGraphEdge<string>>(maxLevels: 1)
                .AddStopNodeKey("Node2");

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort(graphContext);

            var compare = new List<IList<string>>
            {
                new List<string> { "Node1" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void SimpleTopological5ContextTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphEdge<string>("Node1", "Node2"),
                new GraphEdge<string>("Node2", "Node3"),
            };

            var graphContext = new GraphTopologicalContext<String, IGraphEdge<string>>(maxLevels: 1)
                .AddStopNodeKey("Node2");

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort(graphContext);

            var compare = new List<IList<string>>
            {
                new List<string> { "Node1" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void SimpleTopological6ContextTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),
                new GraphEdge<string>("Node1", "Node2"),
                new GraphEdge<string>("Node2", "Node3"),
                new GraphEdge<string>("Node3", "Node4"),
            };

            var graphContext = new GraphTopologicalContext<String, IGraphEdge<string>>(maxLevels: 1)
                .AddStopNodeKey("Node3");

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort(graphContext);

            var compare = new List<IList<string>>
            {
                new List<string> { "Node1" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void SimpleTopological7ContextTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),
                new GraphEdge<string>("Node1", "Node2"),
                new GraphEdge<string>("Node2", "Node3"),
                new GraphEdge<string>("Node3", "Node4"),
            };

            var graphContext = new GraphTopologicalContext<String, IGraphEdge<string>>(maxLevels: 2)
                .AddStopNodeKey("Node3");

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort(graphContext);

            var compare = new List<IList<string>>
            {
                new List<string> { "Node1" },
                new List<string> { "Node2" },
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
