using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public static class GraphExtensionsNodes
    {
        /// <summary>
        /// Get all connecting nodes for a node
        /// </summary>
        /// <param name="key">node's key</param>
        /// <returns>list of leaf children nodes</returns>
        public static IReadOnlyList<TNode> GetLinkedNodes<TKey, TNode, TEdge>(this GraphMap<TKey, TNode, TEdge> self, TKey key)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            var filter = self.CreateFilter()
                .Include(key);

            return self.GetLinkedNodes(filter);
        }

        /// <summary>
        /// Get all the connected nodes for a node(s), including all relationships nodes
        /// </summary>
        /// <param name="filter">filter to apply</param>
        /// <returns>list of leaf children nodes</returns>
        public static IReadOnlyList<TNode> GetLinkedNodes<TKey, TNode, TEdge>(this GraphMap<TKey, TNode, TEdge> self, IGraphFilter<TKey> filter)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            filter.VerifyNotNull(nameof(filter));
            filter.IncludeNodeKeys.Count.VerifyAssert(x => x > 0, "must be at lease 1");

            HashSet<TKey>? excludeKeys = null;

            // If there are exclude node keys, need to get this list first to exclude
            if (filter.ExcludeNodeKeys?.Count > 0)
            {
                var filterKeys = filter.ExcludeNodeKeys
                    .ToHashSet(self.KeyCompare);

                IReadOnlyList<TNode> excludeNodes = self.GetLinkedNodes(filterKeys, filter.IncludeLinkedNodes, null, false, true);

                excludeKeys = excludeNodes
                    .Select(x => x.Key)
                    .Concat(filter.ExcludeNodeKeys)
                    .ToHashSet(self.KeyCompare);
            }

            var focusedKeys = filter.IncludeNodeKeys
                .ToHashSet(self.KeyCompare);

            IReadOnlyList<TNode> result = self.GetLinkedNodes(focusedKeys, filter.IncludeLinkedNodes, excludeKeys, true, false);

            if (filter.IncludeDependentNodes)
            {
                IReadOnlyList<TNode> dependentLinks = self.GetLinkedNodes(focusedKeys, filter.IncludeLinkedNodes, excludeKeys, false, true);

                result = result
                    .Concat(dependentLinks)
                    .GroupBy(x => x.Key, self.KeyCompare)
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
        public static IReadOnlyList<TNode> GetLinkedNodes<TKey, TNode, TEdge>(this GraphMap<TKey, TNode, TEdge> self, HashSet<TKey> includeNodeKeys, bool includeDependentNodes, HashSet<TKey>? excludedNodeKeys, bool ownEdge, bool dependsOnEdge)
            where TNode : IGraphNode<TKey>
            where TEdge : IGraphEdge<TKey>
        {
            includeNodeKeys.VerifyNotNull(nameof(includeNodeKeys));

            var visitedKeys = excludedNodeKeys ?? new HashSet<TKey>(self.KeyCompare);
            var childrenKeys = new HashSet<TKey>(self.KeyCompare);
            var focusedKeys = new List<TKey>(includeNodeKeys);

            bool IsOwnEdge(IGraphEdge<TKey> edge) => ownEdge && typeof(GraphEdge<TKey>).IsAssignableFrom(edge.GetType());
            bool IsDependsOnEdge(IGraphEdge<TKey> edge) => dependsOnEdge && typeof(GraphDependOnEdge<TKey>).IsAssignableFrom(edge.GetType());

            var focusedEdges = includeDependentNodes ?
                self.Edges.Values.Where(x => IsOwnEdge(x) || IsDependsOnEdge(x)).ToList() :
                Enumerable.Empty<TEdge>().ToList();

            while (true)
            {
                var children = focusedKeys
                    .Where(x => !visitedKeys.Contains(x))
                    .Join(focusedEdges, x => x, x => !dependsOnEdge ? x.FromNodeKey : x.ToNodeKey, (o, i) => i, self.KeyCompare)
                    .ToList();

                if (children.Count == 0)
                {
                    return childrenKeys
                        .Concat(focusedKeys.Where(x => !includeNodeKeys.Contains(x)))
                        .Distinct(self.KeyCompare)
                        .Join(self.Nodes.Values, x => x, x => x.Key, (o, i) => i, self.KeyCompare)
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

    }
}
