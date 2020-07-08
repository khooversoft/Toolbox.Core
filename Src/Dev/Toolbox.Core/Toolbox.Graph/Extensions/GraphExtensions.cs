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
        /// Get all the leaf children for a specific node based on edges, breadth-first traversal 
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <typeparam name="TNode">node type</typeparam>
        /// <typeparam name="TEdge">edge type</typeparam>
        /// <param name="self">graph</param>
        /// <param name="node">node to search from</param>
        /// <param name="notInclude">nodes to stop scanning</param>
        /// <returns>list of leaf children nodes</returns>
        public static IReadOnlyList<TNode> GetLeafNodes<TKey, TNode, TEdge>(this GraphMap<TKey, TNode, TEdge> self, TNode node, params TNode[] notInclude)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            self.VerifyNotNull(nameof(self));
            node.VerifyNotNull(nameof(node));

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
        public static IReadOnlyList<KeyValuePair<TNode, IList<TEdge>>> GetNodeByEdges<TKey, TNode, TEdge>(this GraphMap<TKey, TNode, TEdge> self)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            self.VerifyNotNull(nameof(self));

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
        public static GraphFilter<TKey> CreateFilter<TKey, TNode, TEdge>(this GraphMap<TKey, TNode, TEdge> self)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            self.VerifyNotNull(nameof(self));

            return new GraphFilter<TKey>(self.KeyCompare);
        }

        /// <summary>
        /// Create a new graph based on a filter.  Only edges based on "GraphDependsOnEdge" of TEdge
        /// will be included.
        /// </summary>
        /// <typeparam name="TResult">result graph</typeparam>
        /// <param name="filter">filter to create new graph by</param>
        /// <param name="factory">factory to create new graph, if null will use reflection to create</param>
        /// <returns>new graph based on filter</returns>
        public static GraphMap<TKey, TNode, TEdge> Create<TKey, TNode, TEdge>(this GraphMap<TKey, TNode, TEdge> self, IGraphFilter<TKey> filter)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            filter.VerifyNotNull(nameof(filter));

            // Create new graph
            GraphMap<TKey, TNode, TEdge> newGraph = self.Create();

            // Get connected nodes
            IReadOnlyList<TNode> childrenNodes = self.GetLinkedNodes(filter);

            // Add include nodes to the list
            var focusedNodes = filter.IncludeNodeKeys
                .Join(self.Nodes.Values, x => x, x => x.Key, (o, i) => i, self.KeyCompare)
                .Concat(childrenNodes)
                .GroupBy(x => x.Key, self.KeyCompare)
                .Select(x => x.First());

            // Set nodes
            focusedNodes.ForEach(x => newGraph.Add(x));

            // Get edges for nodes
            var focusedEdges = self.Edges.Values
                .Where(x => newGraph.Nodes.ContainsKey(x.FromNodeKey) && newGraph.Nodes.ContainsKey(x.ToNodeKey));

            focusedEdges.ForEach(x => newGraph.Add(x));

            return newGraph;
        }
    }
}
