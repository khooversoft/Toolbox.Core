// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Khooversoft.Toolbox.Standard;

namespace KHooversoft.Toolbox.Graph
{
    /// <summary>
    /// Graph container supports nodes and directed node edges.
    /// 
    /// Circular dependency are not allowed and will raise an exception if added
    /// </summary>
    public class GraphMap<TKey, TNode, TEdge> : IEnumerable<IGraphCommon>
        where TNode : IGraphNode<TKey>
        where TEdge : IGraphEdge<TKey>
    {
        private readonly Dictionary<TKey, TNode> _nodes;
        private readonly Dictionary<Guid, TEdge> _edges;
        private bool _strict;

        public GraphMap(bool strict = true, IEqualityComparer<TKey>? equalityComparer = null)
        {
            KeyCompare = equalityComparer ??
                ((typeof(TKey) == typeof(string)) ? (IEqualityComparer<TKey>)StringComparer.OrdinalIgnoreCase : EqualityComparer<TKey>.Default);

            _strict = strict;
            _nodes = new Dictionary<TKey, TNode>(KeyCompare);
            _edges = new Dictionary<Guid, TEdge>();
        }

        public GraphMap(GraphMap<TKey, TNode, TEdge> graphMap)
            : this(true)
        {
            graphMap.VerifyNotNull(nameof(graphMap));

            graphMap.Nodes.Values.ForEach(x => Add(x));
            graphMap.Edges.Values.ForEach(x => Add(x));
        }

        /// <summary>
        /// Key comparer
        /// </summary>
        public IEqualityComparer<TKey> KeyCompare { get; }

        /// <summary>
        /// Nodes in graph
        /// </summary>
        public IReadOnlyDictionary<TKey, TNode> Nodes => _nodes;

        /// <summary>
        /// Edges in graph
        /// </summary>
        public IReadOnlyDictionary<Guid, TEdge> Edges => _edges;

        /// <summary>
        /// IsStrict operational setting, true and edges are check against node, false
        /// the check is relaxed.
        /// 
        /// When set to true, the edges will be validated
        /// </summary>
        public bool IsStrict
        {
            get { return _strict; }
            set
            {
                bool old = _strict;
                _strict = value;

                if (_strict && !old)
                {
                    ValidateEdgesInternal();
                }
            }
        }

        /// <summary>
        /// Help to create a new instance of the Graph Map
        /// </summary>
        /// <returns>new graph map with strict and key compare set</returns>
        public GraphMap<TKey, TNode, TEdge> Create()
        {
            return new GraphMap<TKey, TNode, TEdge>(_strict, KeyCompare);
        }

        /// <summary>
        /// Set strict operation setting
        /// </summary>
        /// <param name="strict">strict setting</param>
        /// <returns>this</returns>
        public GraphMap<TKey, TNode, TEdge> SetStrict(bool strict)
        {
            IsStrict = strict;
            return this;
        }

        /// <summary>
        /// Add node
        /// </summary>
        /// <param name="node">node</param>
        /// <returns>this</returns>
        public GraphMap<TKey, TNode, TEdge> Add(TNode node)
        {
            _nodes.Add(node.Key, node);
            return this;
        }

        /// <summary>
        /// Add edge
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public GraphMap<TKey, TNode, TEdge> Add(TEdge edge)
        {
            ValidateEdge(edge);

            _edges.Add(edge.Key, edge);

            if (IsStrict)
            {
                IList<IList<TNode>> cycles = FindCycles();
                Verify.Assert(cycles.Count == 0, $"Circular dependency detected for nodes: {string.Join(", ", cycles.SelectMany(x => x))}");
            }

            return this;
        }

        /// <summary>
        /// Remove node, will remove all edges associated
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <returns></returns>
        public GraphMap<TKey, TNode, TEdge> RemoveNode(TKey nodeKey)
        {
            // Remove edges associated with node
            _edges.Values
                .Where(x => KeyCompare.Equals(x.FromNodeKey, nodeKey) || KeyCompare.Equals(x.ToNodeKey, nodeKey))
                .Select(x => x.Key)
                .ToList()
                .ForEach(x => _edges.Remove(x));

            _nodes.Remove(nodeKey);

            return this;
        }

        /// <summary>
        /// Remove edge
        /// </summary>
        /// <param name="fromNodeKey">from node key</param>
        /// <param name="toNodeKey">to node key</param>
        /// <returns></returns>
        public GraphMap<TKey, TNode, TEdge> RemoveEdge(TKey fromNodeKey, TKey toNodeKey)
        {
            Guid? edgeKey = _edges.Values
                .Where(x => KeyCompare.Equals(x.FromNodeKey, fromNodeKey) == true && KeyCompare.Equals(x.ToNodeKey, toNodeKey))
                .Select(x => x.Key)
                .FirstOrDefault();

            if (edgeKey != null)
            {
                _edges.Remove((Guid)edgeKey);
            }

            return this;
        }

        /// <summary>
        /// Return edges for a node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IReadOnlyList<TEdge> GetEdgesForNode(TNode node) =>
            _edges.Values
            .Where(x => KeyCompare.Equals(x.FromNodeKey, node.Key))
            .ToList();

        /// <summary>
        /// Get connected nodes for node specified (directed)
        /// </summary>
        /// <param name="node">node to search for</param>
        /// <returns>list of connected nodes</returns>
        public IReadOnlyList<TNode> GetConnectedNodes(TNode node) => _edges.Values
                .Where(x => KeyCompare.Equals(x.FromNodeKey, node.Key))
                .Join(_nodes.Values, x => x.ToNodeKey, x => x.Key, (o, i) => i)
                .ToList();

        /// <summary>
        /// Find cycles where edges create a loop(s)
        /// </summary>
        /// <returns>return list of nodes that are involved in the loop</returns>
        public IList<IList<TNode>> FindCycles()
        {
            return Nodes.Values.FindCycles<TNode>(n => GetConnectedNodes(n));
        }

        /// <summary>
        /// Return list of nodes by key(s)
        /// </summary>
        /// <param name="nodeKeys">list of node keys</param>
        /// <returns>nodes matching set</returns>
        public IReadOnlyList<TNode> FindNodesByKey(IEnumerable<TKey> nodeKeys)
        {
            if (nodeKeys == null || !nodeKeys.Any())
            {
                return new List<TNode>();
            }

            return Nodes.Values
                .Where(x => nodeKeys.Any(y => KeyCompare.Equals(x.Key, y)))
                .ToList();
        }

        /// <summary>
        /// Validate edges
        /// </summary>
        /// <returns>return edges and error message</returns>
        public IReadOnlyList<(TEdge Edge, string ErrorMsg)> ValidateEdges()
        {
            var list = new List<(TEdge Edge, string ErrorMsg)>();

            foreach (var edge in _edges.Values)
            {
                string? errorMessage = TestEdge(edge);
                if (errorMessage != null)
                {
                    list.Add((edge, errorMessage));
                }
            }

            return list;
        }

        /// <summary>
        /// Validate edges, insure to and from node are valid
        /// </summary>
        private void ValidateEdgesInternal()
        {
            foreach (var edge in _edges.Values)
            {
                ValidateEdge(edge);
            }
        }

        /// <summary>
        /// Validate edge, check to and from node references
        /// </summary>
        /// <param name="edge">edge</param>
        private void ValidateEdge(TEdge edge)
        {
            if (!IsStrict)
            {
                return;
            }

            string? errorMessage = TestEdge(edge);
            if (errorMessage == null)
            {
                return;
            }

            throw new ArgumentException(errorMessage);
        }

        /// <summary>
        /// Test edge and return error message
        /// </summary>
        /// <param name="edge"></param>
        /// <returns>error message</returns>
        private string? TestEdge(TEdge edge)
        {
            if (!_nodes.ContainsKey(edge.FromNodeKey))
            {
                return $"Parent {edge.FromNodeKey} is not found in nodes";
            }

            if (!_nodes.ContainsKey(edge.ToNodeKey))
            {
                return $"Child {edge.ToNodeKey} is not found in nodes";
            }

            if (KeyCompare.Equals(edge.FromNodeKey, edge.ToNodeKey))
            {
                return $"From node {edge.FromNodeKey} cannot be the same as the to {edge.ToNodeKey}.";
            }

            return null;
        }

        /// <summary>
        /// Enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<IGraphCommon> GetEnumerator() => _nodes.Values.OfType<IGraphCommon>().GetEnumerator();

        /// <summary>
        /// Enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
