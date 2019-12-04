// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public interface IReadOnlyGraphMap<TKey, TNode, TEdge> : IEnumerable<IGraphCommon>
        where TNode : IGraphNode<TKey>
        where TEdge : IGraphEdge<TKey>
    {
        IEqualityComparer<TKey> KeyCompare { get; }

        IReadOnlyDictionary<TKey, TNode> Nodes { get; }

        IReadOnlyDictionary<Guid, TEdge> Edges { get; }

        bool IsStrict { get; }

        IReadOnlyList<TNode> GetConnectedNodes(TNode node);

        IReadOnlyList<TNode> GetLinkedNodes(IGraphFilter<TKey> filter);

        IReadOnlyList<TNode> GetLinkedNodes(TKey key);

        GraphMap<TKey, TNode, TEdge> Create();
    }
}
