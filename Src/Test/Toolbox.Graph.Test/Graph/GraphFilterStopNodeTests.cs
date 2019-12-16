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
    public class GraphFilterStopNodeTests
    {
        [Fact]
        public void SingleStopNodeTest()
        {
            var map = CreateMap();

            var filter = map.CreateFilter()
                .Include("Workflow")
                .Exclude("PostCodes-A");

            var filterMap = map.Create(filter);

            IList<IList<IGraphNode<string>>> sort = filterMap.TopologicalSort();

            var result = new string[][]
            {
                new string[] { "Workflow" },
                new string[] { "Admin-A", "BaseMap-A" },
                new string[] { "Admin-P", "BaseMap-P" },
                new string[] { "Admin-P-A1", "Admin-P-A2", "Admin-P-A3", "BaseMap-P-A1" },
                new string[] { "BaseMap-P-A2" },
                new string[] { "BaseMap-P-A3" },
            };

            Verify(sort, result);
        }

        [Fact]
        public void TwoStopNodeTest()
        {
            var map = CreateMap();

            var filter = map.CreateFilter()
                .Include("Workflow")
                .Exclude("PostCodes-A", "BaseMap-A");

            var filterMap = map.Create(filter);

            IList<IList<IGraphNode<string>>> sort = filterMap.TopologicalSort();

            var result = new string[][]
            {
                new string[] { "Workflow" },
                new string[] { "Admin-A" },
                new string[] { "Admin-P" },
                new string[] { "Admin-P-A1", "Admin-P-A2", "Admin-P-A3" },
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
    }
}
