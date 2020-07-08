using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public static class GraphExtensionsTopological
    {
        /// <summary>
        /// Full default topological sort
        /// </summary>
        /// <returns>List of list of nodes</returns>
        public static IList<IList<TNode>> TopologicalSort<TKey, TNode, TEdge>(this GraphMap<TKey, TNode, TEdge> self)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            return self.TopologicalSort(new GraphTopologicalContext<TKey, TEdge>(self.KeyCompare));
        }

        /// <summary>
        /// <para>Return a list representing a topological sort based on edges is defined by a directed graph.</para>
        /// <para>
        /// The first item in the return list will be nodes with no edges.  The next item (if there is one)
        /// will be nodes that only have dependencies on the previous list items, and so on.
        /// 
        /// Uses two list to keep track of what nodes have been processed and stopped nodes.  Stopped nodes
        /// stop the sorting for that edge(s).
        /// </para>
        /// </summary>
        /// <param name="graphContext">Graph topological context</param>
        /// <returns>List of list of nodes</returns>
        public static IList<IList<TNode>> TopologicalSort<TKey, TNode, TEdge>(this GraphMap<TKey, TNode, TEdge> self, GraphTopologicalContext<TKey, TEdge> graphContext)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            self.VerifyNotNull(nameof(self));
            graphContext.VerifyNotNull(nameof(graphContext));

            var visit = new HashSet<TKey>(graphContext.ProcessedNodeKeys, self.KeyCompare);
            var stopNodes = new HashSet<TKey>(graphContext.StopNodeKeys, self.KeyCompare);
            var orderList = new List<IList<TNode>>();

            var nodeCounts = new List<(TNode Node, int Count)>();

            static bool TestEdge(Type type, TEdge edge) => type.IsInterface ? type.IsAssignableFrom(edge!.GetType()) : edge!.GetType() == type;

                IReadOnlyList<TEdge> edgesToUse = self.Edges.Values
                .Where(x => TestEdge(graphContext.EdgeType, x))
                .ToList();

            while (true)
            {
                nodeCounts.Clear();

                var nodesToCount = self.Nodes.Values.Where(x => !visit.Contains(x.Key) && !stopNodes.Contains(x.Key))
                        .ToList();

                if (nodesToCount.Count == 0)
                {
                    return orderList;
                }

                foreach (var node in nodesToCount)
                {
                    // Count forward (own) edges
                    int forwardCount = edgesToUse
                        .OfType<GraphEdge<TKey>>()
                        .Where(x => !visit.Contains(x.FromNodeKey))
                        .Where(x => self.KeyCompare.Equals(x.ToNodeKey, node.Key))
                        .Count();

                    int dependOnCount = edgesToUse
                        .OfType<GraphDependOnEdge<TKey>>()
                        .Where(x => !visit.Contains(x.ToNodeKey))
                        .Where(x => self.KeyCompare.Equals(x.FromNodeKey, node.Key))
                        .Count();

                    var phase1 = edgesToUse
                        .OfType<GraphDependOnEdge<TKey>>()
                        .Where(x => self.KeyCompare.Equals(x.FromNodeKey, node.Key)).ToList();

                    var phase2 = phase1
                        .SelectMany(x => self.GetLinkedNodes(self.CreateFilter().Include(x.ToNodeKey))).ToList();

                    var phase3 = phase2
                        .Where(x => !visit.Contains(x.Key)).ToList();

                    int depthDependsOnCount = phase3
                        .Count();

                    nodeCounts.Add((node, forwardCount + dependOnCount + depthDependsOnCount));
                }

                var zero = nodeCounts
                    .Where(x => x.Count == 0)
                    .Select(x => x.Node)
                    .ToList();

                if (zero.Count == 0)
                {
                    return orderList;
                }

                orderList.Add(zero);

                if (orderList.Count == graphContext.MaxLevels)
                {
                    return orderList;
                }

                zero.ForEach(x => visit.Add(x.Key));
            }
        }
    }
}
