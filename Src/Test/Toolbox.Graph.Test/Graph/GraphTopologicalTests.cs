using FluentAssertions;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Graph.Test
{
    public class GraphTopologicalTests
    {
        [Fact]
        public void NoTopologicalTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
            };

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();
            sort.Count.Should().Be(1);
            sort[0].Count.Should().Be(2);
        }

        [Fact]
        public void SimpleTopologicalTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphEdge<string>("Node1", "Node2"),
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
        public void TwoLevelTopologicalTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphEdge<string>("Node2", "Node1"),
                new GraphEdge<string>("Node3", "Node2"),
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
        public void ThreeLevelTopologicalTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),
                new GraphEdge<string>("Node2", "Node1"),
                new GraphEdge<string>("Node3", "Node2"),
                new GraphEdge<string>("Node4", "Node3"),
            };


            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "Node4" },
                new List<string> { "Node3" },
                new List<string> { "Node2" },
                new List<string> { "Node1" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void TreeTwoLevelTopologicalTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2"),
                new GraphNode<string>("Node3"),
                new GraphNode<string>("Node4"),
                new GraphEdge<string>("Node3", "Node2"),
                new GraphEdge<string>("Node4", "Node2"),
            };


            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "Node1", "Node3", "Node4" },
                new List<string> { "Node2" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void SimpleTreeTopologicalTest()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2-1"),
                new GraphNode<string>("Node2-2"),
                new GraphEdge<string>("Node2-1", "Node1"),
                new GraphEdge<string>("Node2-2", "Node1"),
            };


            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "Node2-1", "Node2-2" },
                new List<string> { "Node1" },
            };

            Verify(sort, compare);
        }

        [Fact]
        public void SimpleTreeTopological2Test()
        {
            var map = new GraphMap<string, IGraphNode<string>, IGraphEdge<string>>()
            {
                new GraphNode<string>("Node1"),
                new GraphNode<string>("Node2-1"),
                new GraphNode<string>("Node2-2"),
                new GraphNode<string>("Node3-1"),
                new GraphNode<string>("Node3-2"),
                new GraphEdge<string>("Node2-1", "Node1"),
                new GraphEdge<string>("Node2-2", "Node1"),
                new GraphEdge<string>("Node3-1", "Node2-1"),
                new GraphEdge<string>("Node3-2", "Node2-2"),
            };


            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var compare = new List<IList<string>>
            {
                new List<string> { "Node3-1", "Node3-2" },
                new List<string> { "Node2-1", "Node2-2" },
                new List<string> { "Node1" },
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
