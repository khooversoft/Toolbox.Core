using FluentAssertions;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Graph.Test
{
    public class GraphTopological2Tests
    {
        [Fact]
        public void SimpleGraphTest()
        {
            var builder = new GraphMapBuilder<string, IGraphNode<string>, IGraphEdge<string>>
            {
                new NodeMap<string>("Workflow")
                {
                    new NodeMap<string>("PostCodes-A")
                    {
                        new NodeMap<string>("PostCode-P")
                        {
                            new NodeMap<string>("PostCode-P-A1"),
                            new NodeMap<string>("PostCode-P-A2"),
                        }
                    },

                    new NodeMap<string>("Admin-A")
                    {
                        new NodeMap<string>("Admin-P")
                        {
                            new NodeMap<string>("Admin-P-A1"),
                            new NodeMap<string>("Admin-P-A2"),
                            new NodeMap<string>("Admin-P-A3"),
                        }
                    },

                    new NodeMap<string>("BaseMap-A")
                    {
                        new NodeMap<string>("BaseMap-P")
                        {
                            new SequenceMap<string>()
                            {
                                new NodeMap<string>("BaseMap-P-A1"),
                                new NodeMap<string>("BaseMap-P-A2"),
                                new NodeMap<string>("BaseMap-P-A3"),
                            }
                        }
                    },
                }
            };

            var map = builder.Build<GraphMap<string, IGraphNode<string>, IGraphEdge<string>>>();

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var result = new string[][]
            {
                new string[] { "Workflow" },
                new string[] { "PostCodes-A", "Admin-A", "BaseMap-A" },
                new string[] { "PostCode-P", "Admin-P", "BaseMap-P" },
                new string[] { "PostCode-P-A1", "PostCode-P-A2", "Admin-P-A1", "Admin-P-A2", "Admin-P-A3", "BaseMap-P-A1" },
                new string[] { "BaseMap-P-A2" },
                new string[] { "BaseMap-P-A3" },
            };

            Verify(sort, result);
        }

        [Fact]
        public void SimpleOrderingGraphTest()
        {
            var builder = new GraphMapBuilder<string, IGraphNode<string>, IGraphEdge<string>>
            {
                new NodeMap<string>("Workflow")
                {
                    new NodeMap<string>("PostCodes-A")
                    {
                        new NodeMap<string>("PostCode-P")
                        {
                            new NodeMap<string>("PostCode-P-A1"),
                            new NodeMap<string>("PostCode-P-A2"),
                        }
                    },

                    new NodeMap<string>("Admin-A")
                    {
                        new NodeMap<string>("Admin-P")
                        {
                            new NodeMap<string>("Admin-P-A1"),
                            new NodeMap<string>("Admin-P-A2"),
                            new NodeMap<string>("Admin-P-A3"),
                        }
                    },

                    new NodeMap<string>("BaseMap-A")
                    {
                        new NodeMap<string>("BaseMap-P")
                        {
                            new SequenceMap<string>()
                            {
                                new NodeMap<string>("BaseMap-P-A1"),
                                new NodeMap<string>("BaseMap-P-A2"),
                                new NodeMap<string>("BaseMap-P-A3"),
                            }
                        }
                    },
                },

                new GraphDependOnEdge<string>("BaseMap-A", "PostCodes-A")
            };

            var map = builder.Build<GraphMap<string, IGraphNode<string>, IGraphEdge<string>>>();

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var result = new string[][]
            {
                new string[] { "Workflow" },
                new string[] { "PostCodes-A", "Admin-A" },
                new string[] { "PostCode-P", "Admin-P" },
                new string[] { "PostCode-P-A1", "PostCode-P-A2", "Admin-P-A1", "Admin-P-A2", "Admin-P-A3" },
                new string[] { "BaseMap-A" },
                new string[] { "BaseMap-P" },
                new string[] { "BaseMap-P-A1" },
                new string[] { "BaseMap-P-A2" },
                new string[] { "BaseMap-P-A3" },
            };

            Verify(sort, result);
        }

        [Fact]
        public void ComplexOrderTest()
        {
            var map = CreateMap();

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var result = new string[][]
            {
                new string[] { "Workflow" },
                new string[] { "PostCodes-A", "Admin-A", "BaseMap-A" },
                new string[] { "PostCode-P", "Admin-P", "BaseMap-P" },
                new string[] { "PostCode-P-A1", "PostCode-P-A2", "Admin-P-A1", "Admin-P-A2", "Admin-P-A3", "BaseMap-P-A1" },
                new string[] { "BaseMap-P-A2" },
                new string[] { "BaseMap-P-A3" },
                new string[] { "DC-A" },
                new string[] { "DC-P" },
                new string[] { "DC-P-A1" },
                new string[] { "DC-P-A2" },
                new string[] { "DC-P-A3" },
                new string[] { "DC-P-A4" },
                new string[] { "KOFMetrics-A" },
                new string[] { "KOFMetrics-P" },
                new string[] { "KOFMetrics-P-A1", "KOFMetrics-P-A2", "KOFMetrics-P-A3", "KOFMetrics-P-A4" }
            };

            Verify(sort, result);
        }
        
        [Fact]
        public void FilterOutExtraPipelinesTest()
        {
            var mapPrime = CreateMap2();
            var map = mapPrime.Create(mapPrime.CreateFilter().Include("Workflow"));

            IList<IList<IGraphNode<string>>> sort = map.TopologicalSort();

            var result = new string[][]
            {
                new string[] { "Workflow" },
                new string[] { "PostCodes-A", "BaseMap-A" },
                new string[] { "PostCode-P", "BaseMap-P" },
                new string[] { "PostCode-P-A1", "PostCode-P-A2", "BaseMap-P-A1" },
                new string[] { "BaseMap-P-A2" },
                new string[] { "BaseMap-P-A3" },
                new string[] { "DC-A" },
                new string[] { "DC-P" },
                new string[] { "DC-P-A1" },
                new string[] { "DC-P-A2" },
                new string[] { "DC-P-A3" },
                new string[] { "DC-P-A4" },
                new string[] { "KOFMetrics-A" },
                new string[] { "KOFMetrics-P" },
                new string[] { "KOFMetrics-P-A1", "KOFMetrics-P-A2", "KOFMetrics-P-A3", "KOFMetrics-P-A4" }
            };

            Verify(sort, result);
        }

        private void Verify(IList<IList<IGraphNode<string>>> sort, string[][] result)
        {
            sort.Count.Should().Be(result.Length);

            int sortRow = 0;
            foreach (var row in result)
            {
                row.Length.Should().Be(sort[sortRow].Count);
                row.OrderBy(x => x)
                    .Zip(sort[sortRow].OrderBy(x => x.Key), (o, i) => new { o, i })
                    .All(x => x.o == x.i.Key)
                    .Should().BeTrue($"Row# {sortRow}");

                sortRow++;
            }
        }

        private GraphMap<string, IGraphNode<string>, IGraphEdge<string>> CreateMap()
        {
            return new GraphMapBuilder<string, IGraphNode<string>, IGraphEdge<string>>
            {
                new NodeMap<string>("Workflow")
                {
                    new NodeMap<string>("PostCodes-A")
                    {
                        new NodeMap<string>("PostCode-P")
                        {
                            new NodeMap<string>("PostCode-P-A1"),
                            new NodeMap<string>("PostCode-P-A2"),
                        }
                    },

                    new NodeMap<string>("Admin-A")
                    {
                        new NodeMap<string>("Admin-P")
                        {
                            new NodeMap<string>("Admin-P-A1"),
                            new NodeMap<string>("Admin-P-A2"),
                            new NodeMap<string>("Admin-P-A3"),
                        }
                    },

                    new NodeMap<string>("BaseMap-A")
                    {
                        new NodeMap<string>("BaseMap-P")
                        {
                            new SequenceMap<string>()
                            {
                                new NodeMap<string>("BaseMap-P-A1"),
                                new NodeMap<string>("BaseMap-P-A2"),
                                new NodeMap<string>("BaseMap-P-A3"),
                            }
                        }
                    },

                    new NodeMap<string>("DC-A")
                    {
                        new NodeMap<string>("DC-P")
                        {
                            new SequenceMap<string>()
                            {
                                new NodeMap<string>("DC-P-A1"),
                                new NodeMap<string>("DC-P-A2"),
                                new NodeMap<string>("DC-P-A3"),
                                new NodeMap<string>("DC-P-A4"),
                            }
                        }
                    },

                    new NodeMap<string>("KOFMetrics-A")
                    {
                        new NodeMap<string>("KOFMetrics-P")
                        {
                            new NodeMap<string>("KOFMetrics-P-A1"),
                            new NodeMap<string>("KOFMetrics-P-A2"),
                            new NodeMap<string>("KOFMetrics-P-A3"),
                            new NodeMap<string>("KOFMetrics-P-A4"),
                        }
                    }
                },

                new GraphDependOnEdge<string>("DC-A", "PostCodes-A"),
                new GraphDependOnEdge<string>("DC-A", "Admin-A"),
                new GraphDependOnEdge<string>("DC-A", "BaseMap-A"),
                new GraphDependOnEdge<string>("DC-A", "PostCodes-A"),

                new GraphDependOnEdge<string>("KOFMetrics-A", "DC-A"),

            }.Build<GraphMap<string, IGraphNode<string>, IGraphEdge<string>>>();
        }

        private GraphMap<string, IGraphNode<string>, IGraphEdge<string>> CreateMap2()
        {
            return new GraphMapBuilder<string, IGraphNode<string>, IGraphEdge<string>>
            {
                new NodeMap<string>("Workflow")
                {
                    new NodeMap<string>("PostCodes-A")
                    {
                        new NodeMap<string>("PostCode-P")
                        {
                            new NodeMap<string>("PostCode-P-A1"),
                            new NodeMap<string>("PostCode-P-A2"),
                        }
                    },

                    new NodeMap<string>("BaseMap-A")
                    {
                        new NodeMap<string>("BaseMap-P")
                        {
                            new SequenceMap<string>()
                            {
                                new NodeMap<string>("BaseMap-P-A1"),
                                new NodeMap<string>("BaseMap-P-A2"),
                                new NodeMap<string>("BaseMap-P-A3"),
                            }
                        }
                    },

                    new NodeMap<string>("DC-A")
                    {
                        new NodeMap<string>("DC-P")
                        {
                            new SequenceMap<string>()
                            {
                                new NodeMap<string>("DC-P-A1"),
                                new NodeMap<string>("DC-P-A2"),
                                new NodeMap<string>("DC-P-A3"),
                                new NodeMap<string>("DC-P-A4"),
                            }
                        }
                    },

                    new NodeMap<string>("KOFMetrics-A")
                    {
                        new NodeMap<string>("KOFMetrics-P")
                        {
                            new NodeMap<string>("KOFMetrics-P-A1"),
                            new NodeMap<string>("KOFMetrics-P-A2"),
                            new NodeMap<string>("KOFMetrics-P-A3"),
                            new NodeMap<string>("KOFMetrics-P-A4"),
                        }
                    }
                },

                new NodeMap<string>("Admin-A")
                {
                    new NodeMap<string>("Admin-P")
                    {
                        new NodeMap<string>("Admin-P-A1"),
                        new NodeMap<string>("Admin-P-A2"),
                        new NodeMap<string>("Admin-P-A3"),
                    }
                },

                new GraphDependOnEdge<string>("DC-A", "PostCodes-A"),
                new GraphDependOnEdge<string>("DC-A", "Admin-A"),
                new GraphDependOnEdge<string>("DC-A", "BaseMap-A"),
                new GraphDependOnEdge<string>("DC-A", "PostCodes-A"),

                new GraphDependOnEdge<string>("KOFMetrics-A", "DC-A"),

            }.Build<GraphMap<string, IGraphNode<string>, IGraphEdge<string>>>();
        }
    }
}
