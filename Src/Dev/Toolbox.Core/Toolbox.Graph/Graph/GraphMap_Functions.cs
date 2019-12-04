// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KHooversoft.Toolbox.Graph
{
    public partial class GraphMap<TKey, TNode, TEdge> : IReadOnlyGraphMap<TKey, TNode, TEdge>
    {
        /// <summary>
        /// Get all connecting nodes for a node
        /// </summary>
        /// <param name="key">node's key</param>
        /// <returns>list of leaf children nodes</returns>
        public IReadOnlyList<TNode> GetLinkedNodes(TKey key)
        {
            var filter = this.CreateFilter()
                .Include(key);

            return GetLinkedNodes(filter);
        }

        /// <summary>
        /// Get all the connected nodes for a node(s), including all relationships nodes
        /// </summary>
        /// <param name="filter">filter to apply</param>
        /// <returns>list of leaf children nodes</returns>
        public IReadOnlyList<TNode> GetLinkedNodes(IGraphFilter<TKey> filter)
        {
            filter.Verify(nameof(filter)).IsNotNull();
            filter.IncludeNodeKeys.Count.Verify(nameof(filter.IncludeNodeKeys)).Assert(x => x > 0, "must be at lease 1");

            HashSet<TKey> excludeKeys = null;

            // If there are exclude node keys, need to get this list first to exclude
            if (filter.ExcludeNodeKeys?.Count > 0)
            {
                var filterKeys = filter.ExcludeNodeKeys
                    .ToHashSet(KeyCompare);

                IReadOnlyList<TNode> excludeNodes = GetLinkedNodes(filterKeys, filter.IncludeLinkedNodes, null, false, true);

                excludeKeys = excludeNodes
                    .Select(x => x.Key)
                    .Concat(filter.ExcludeNodeKeys)
                    .ToHashSet(KeyCompare);
            }

            var focusedKeys = filter.IncludeNodeKeys
                .ToHashSet(KeyCompare);

            IReadOnlyList<TNode> result = GetLinkedNodes(focusedKeys, filter.IncludeLinkedNodes, excludeKeys, true, false);

            if (filter.IncludeDependentNodes)
            {
                IReadOnlyList<TNode> dependentLinks = GetLinkedNodes(focusedKeys, filter.IncludeLinkedNodes, excludeKeys, false, true);

                result = result
                    .Concat(dependentLinks)
                    .GroupBy(x => x.Key, KeyCompare)
                    .Select(x => x.First())
                    .ToList();
            }

            return result;
        }

        /// <summary>
        /// Get all connected nodes for include set, excludeNodes will stop the edge tracing.
        /// </summary>
        /// <param name="includeNodeKeys">nodes to focus on</param>
        /// <param name="includeDependentNodes">true to trace dependencies</param>
        /// <param name="excludedNodeKeys">exclude nodes, will stop trace dependencies (edges) on these nodes</param>
        /// <param name="ownEdge">true to trace own edges</param>
        /// <param name="dependsOnEdge">true to trace depends on edges</param>
        /// <returns>List of linked nodes</returns>
        public IReadOnlyList<TNode> GetLinkedNodes(HashSet<TKey> includeNodeKeys, bool includeDependentNodes, HashSet<TKey> excludedNodeKeys, bool ownEdge, bool dependsOnEdge)
        {
            includeNodeKeys.Verify(nameof(includeNodeKeys)).IsNotNull();

            var visitedKeys = excludedNodeKeys ?? new HashSet<TKey>(KeyCompare);
            var childrenKeys = new HashSet<TKey>(KeyCompare);
            var focusedKeys = new List<TKey>(includeNodeKeys);

            bool IsOwnEdge(IGraphEdge<TKey> edge) => ownEdge && typeof(GraphEdge<TKey>).IsAssignableFrom(edge.GetType());
            bool IsDependsOnEdge(IGraphEdge<TKey> edge) => dependsOnEdge && typeof(GraphDependOnEdge<TKey>).IsAssignableFrom(edge.GetType());

            var focusedEdges = includeDependentNodes ?
                Edges.Values.Where(x => IsOwnEdge(x) || IsDependsOnEdge(x)).ToList() :
                Enumerable.Empty<TEdge>().ToList();

            while (true)
            {
                var children = focusedKeys
                    .Where(x => !visitedKeys.Contains(x))
                    .Join(focusedEdges, x => x, x => !dependsOnEdge ? x.FromNodeKey : x.ToNodeKey, (o, i) => i, KeyCompare)
                    .ToList();

                if (children.Count == 0)
                {
                    return childrenKeys
                        .Concat(focusedKeys.Where(x => !includeNodeKeys.Contains(x)))
                        .Distinct(KeyCompare)
                        .Join(Nodes.Values, x => x, x => x.Key, (o, i) => i, KeyCompare)
                        .ToList();
                }

                focusedKeys.Clear();

                children
                    .Select(x => !dependsOnEdge ? x.ToNodeKey : x.FromNodeKey)
                    .Where(x => !visitedKeys.Contains(x))
                    .ForEach(x => focusedKeys.Add(x));

                focusedKeys
                    .ForEach(x => childrenKeys.Add(x));

                children
                    .ForEach(x => visitedKeys.Add(!dependsOnEdge ? x.FromNodeKey : x.ToNodeKey));
            }
        }

        /// <summary>
        /// Create a new default graph based on a filter.Only edges based on "GraphDependsOnEdge" of TEdge
        /// will be included.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>new graph based on filter</returns>
        public GraphMap<TKey, TNode, TEdge> Create(IGraphFilter<TKey> filter)
        {
            return Create(filter, Create);
        }

        /// <summary>
        /// Create a new graph based on a filter.  Only edges based on "GraphDependsOnEdge" of TEdge
        /// will be included.
        /// </summary>
        /// <typeparam name="TResult">result graph</typeparam>
        /// <param name="filter">filter to create new graph by</param>
        /// <param name="factory">factory to create new graph, if null will use reflection to create</param>
        /// <returns>new graph based on filter</returns>
        public TResult Create<TResult>(IGraphFilter<TKey> filter, Func<TResult> factory = null)
            where TResult : GraphMap<TKey, TNode, TEdge>
        {
            filter.Verify(nameof(filter)).IsNotNull();
            factory.Verify(nameof(factory)).IsNotNull();

            // Create new graph
            TResult newGraph = factory?.Invoke() ?? (TResult)Create();

            // Get connected nodes
            IReadOnlyList<TNode> childrenNodes = GetLinkedNodes(filter);

            // Add include nodes to the list
            var focusedNodes = filter.IncludeNodeKeys
                .Join(Nodes.Values, x => x, x => x.Key, (o, i) => i, KeyCompare)
                .Concat(childrenNodes)
                .GroupBy(x => x.Key, KeyCompare)
                .Select(x => x.First());

            // Set nodes
            focusedNodes.ForEach(x => newGraph.Add(x));

            // Get edges for nodes
            var focusedEdges = Edges.Values
                .Where(x => newGraph.Nodes.ContainsKey(x.FromNodeKey) && newGraph.Nodes.ContainsKey(x.ToNodeKey));

            focusedEdges.ForEach(x => newGraph.Add(x));

            return newGraph;
        }
    }
}
