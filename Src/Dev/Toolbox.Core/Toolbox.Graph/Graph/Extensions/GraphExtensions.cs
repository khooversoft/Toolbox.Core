// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public static class GraphExtensions
    {
        /// <summary>
        /// Full default topological sort
        /// </summary>
        /// <returns>List of list of nodes</returns>
        public static IList<IList<TNode>> TopologicalSort<TKey, TNode, TEdge>(this IReadOnlyGraphMap<TKey, TNode, TEdge> self)
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
        public static IList<IList<TNode>> TopologicalSort<TKey, TNode, TEdge>(this IReadOnlyGraphMap<TKey, TNode, TEdge> self, GraphTopologicalContext<TKey, TEdge> graphContext)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            self.Verify(nameof(self)).IsNotNull();
            graphContext.Verify(nameof(graphContext)).IsNotNull();

            var visit = new HashSet<TKey>(graphContext.ProcessedNodeKeys, self.KeyCompare);
            var stopNodes = new HashSet<TKey>(graphContext.StopNodeKeys, self.KeyCompare);
            var orderList = new List<IList<TNode>>();

            var nodeCounts = new List<(TNode Node, int Count)>();

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

        /// <summary>
        /// Get all the leaf children for a specific node based on edges, breadth-first traversal 
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <typeparam name="TNode">node type</typeparam>
        /// <typeparam name="TEdge">edge type</typeparam>
        /// <param name="self">graph</param>
        /// <param name="node">node to search from</param>
        /// <param name="notInclude">nodes to stop scanning</param>
        /// <returns>list of leaf children nodes</returns>
        public static IReadOnlyList<TNode> GetLeafNodes<TKey, TNode, TEdge>(this IReadOnlyGraphMap<TKey, TNode, TEdge> self, TNode node, params TNode[] notInclude)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            self.Verify(nameof(self)).IsNotNull();
            node.Verify(nameof(node)).IsNotNull();

            var leafNodes = new HashSet<TKey>(self.KeyCompare);
            var visitedNodes = new HashSet<TKey>(self.KeyCompare);
            var notIncludesNodes = new HashSet<TKey>(notInclude.Select(x => x.Key), self.KeyCompare);

            var focusedNodes = new List<TKey>
            {
                node.Key
            };

            while (true)
            {
                var children = focusedNodes
                    .Where(x => !visitedNodes.Contains(x))
                    .Join(self.Edges.Values, x => x, x => x.FromNodeKey, (o, i) => i, self.KeyCompare)
                    .ToList();

                if (children.Count == 0)
                {
                    return leafNodes
                        .Concat(focusedNodes.Where(x => !self.KeyCompare.Equals(x, node.Key)))
                        .Distinct(self.KeyCompare)
                        .Join(self.Nodes.Values, x => x, x => x.Key, (o, i) => i, self.KeyCompare)
                        .ToList();
                }

                // focused nodes that have no children are leaf nodes
                focusedNodes.Except(children.Select(x => x.FromNodeKey), self.KeyCompare).ForEach(x => leafNodes.Add(x));

                focusedNodes.Clear();
                focusedNodes.AddRange(children.Select(x => x.ToNodeKey).Where(x => !notIncludesNodes.Contains(x)));
                children.ForEach(x => visitedNodes.Add(x.FromNodeKey));
            }
        }

        /// <summary>
        /// Get nodes by edges, can be used for dumps
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <typeparam name="TNode">node type</typeparam>
        /// <typeparam name="TEdge">edge type</typeparam>
        /// <param name="self">graph map</param>
        /// <returns>list of nodes with its edges</returns>
        public static IReadOnlyList<KeyValuePair<TNode, IList<TEdge>>> GetNodeByEdges<TKey, TNode, TEdge>(this IReadOnlyGraphMap<TKey, TNode, TEdge> self)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            self.Verify(nameof(self)).IsNotNull();

            var fromNodes = self.Nodes.Values
                .Join(self.Edges.Values, x => x.Key, x => x.FromNodeKey, (o, i) => new { Node = o, Edge = i });

            var toNodes = self.Nodes.Values
                .Join(self.Edges.Values, x => x.Key, x => x.ToNodeKey, (o, i) => new { Node = o, Edge = i });

            var result = fromNodes
                .Concat(toNodes)
                .GroupBy(x => x.Node.Key)
                .Select(x => new KeyValuePair<TNode, IList<TEdge>>(self.Nodes[x.Key], x.Select(y => y.Edge).ToList()))
                .ToList();

            return result;
        }

        /// <summary>
        /// Create filter
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <typeparam name="TNode">node type</typeparam>
        /// <typeparam name="TEdge">edge type</typeparam>
        /// <param name="self">graph map reference</param>
        /// <returns>new graph filter</returns>
        public static GraphFilter<TKey> CreateFilter<TKey, TNode, TEdge>(this IReadOnlyGraphMap<TKey, TNode, TEdge> self)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            self.Verify(nameof(self)).IsNotNull();

            return new GraphFilter<TKey>(self.KeyCompare);
        }

        private static bool TestEdge<TEdge>(Type type, TEdge edge)
        {
            return type.IsInterface ? type.IsAssignableFrom(edge.GetType()) : edge.GetType() == type;
        }
    }
}
