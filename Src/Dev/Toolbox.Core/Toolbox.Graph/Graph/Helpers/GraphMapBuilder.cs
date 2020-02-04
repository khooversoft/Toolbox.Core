// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace KHooversoft.Toolbox.Graph
{
    public class GraphMapBuilder<TKey, TNode, TEdge> : IEnumerable<IGraphCommon>
        where TNode : IGraphNode<TKey>
        where TEdge : IGraphEdge<TKey>
    {
        private readonly List<IGraphCommon> _items = new List<IGraphCommon>();

        public GraphMapBuilder()
        {
            CreateNode = x => new GraphNode<TKey>(x);
            CreateEdge = (f, t) => new GraphEdge<TKey>(f, t);
            CreateDependsOnEdge = (f, t) => new GraphDependOnEdge<TKey>(f, t);
        }

        public GraphMapBuilder(Func<TKey, IGraphNode<TKey>> createNode, Func<TKey, TKey, IGraphEdge<TKey>> createEdge, Func<TKey, TKey, IGraphEdge<TKey>> createDepondsEdge)
        {
            createNode.Verify(nameof(createNode)).IsNotNull();
            createEdge.Verify(nameof(createEdge)).IsNotNull();
            createDepondsEdge.Verify(nameof(createDepondsEdge)).IsNotNull();

            CreateNode = createNode;
            CreateEdge = createEdge;
            CreateDependsOnEdge = createDepondsEdge;
        }

        public IReadOnlyList<IGraphCommon> Items => _items;

        public Func<TKey, IGraphNode<TKey>> CreateNode { get; }

        public Func<TKey, TKey, IGraphEdge<TKey>> CreateEdge { get; }

        public Func<TKey, TKey, IGraphEdge<TKey>> CreateDependsOnEdge { get; }

        public GraphMapBuilder<TKey, TNode, TEdge> Add(IGraphCommon item)
        {
            _items.Add(item);
            return this;
        }

        public TResult Build<TResult>(Func<TResult> createGraphMap = null)
            where TResult : GraphMap<TKey, TNode, TEdge>
        {
            TResult map = createGraphMap?.Invoke() ?? (TResult)new GraphMap<TKey, TNode, TEdge>();

            foreach (var item in _items)
            {
                AddItem(map, item, default);
            }

            return map;
        }

        private void AddItem(GraphMap<TKey, TNode, TEdge> map, IGraphCommon element, TKey parentKey)
        {
            switch (element)
            {
                case NodeMap<TKey> node:
                    map.Add((TNode)CreateNode(node.Key));

                    if (!EqualityComparer<TKey>.Default.Equals(parentKey, default(TKey)))
                    {
                        map.Add((TEdge)CreateEdge(parentKey, node.Key));
                    }

                    foreach (var child in node)
                    {
                        AddItem(map, child, node.Key);
                    }
                    break;

                case SequenceMap<TKey> sequence:
                    TKey linkParent = parentKey;
                    bool first = true;

                    foreach (var child in sequence)
                    {
                        AddItem(map, child, parentKey);

                        if (child is IGraphNode<TKey> childNode)
                        {
                            if (!first)
                            {
                                map.Add((TEdge)CreateDependsOnEdge(childNode.Key, linkParent));
                            }
                            else
                            {
                                first = false;
                            }

                            linkParent = childNode.Key;
                        }
                    }

                    break;

                case IGraphNode<TKey> node:
                    map.Add((TNode)CreateNode(node.Key));
                    break;

                case GraphEdge<TKey> edge:
                    map.Add((TEdge)CreateEdge(edge.FromNodeKey, edge.ToNodeKey));
                    break;

                case GraphDependOnEdge<TKey> edge:
                    map.Add((TEdge)CreateDependsOnEdge(edge.FromNodeKey, edge.ToNodeKey));
                    break;

                default:
                    throw new ArgumentException($"Not supported type: {element.GetType()}");
            }
        }

        public IEnumerator<IGraphCommon> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
