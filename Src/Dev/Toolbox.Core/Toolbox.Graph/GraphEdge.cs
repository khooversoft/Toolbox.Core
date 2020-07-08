// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public class GraphEdge<TKey> : IGraphEdge<TKey>
    {
        public GraphEdge(TKey fromNodeKey, TKey toNodeKey)
        {
            FromNodeKey = fromNodeKey;
            ToNodeKey = toNodeKey;
        }

        public Guid Key { get; } = Guid.NewGuid();

        public TKey FromNodeKey { get; }

        public TKey ToNodeKey { get; }

        public override string ToString()
        {
            return $"{this.GetType().Name}, FromNodeKey={FromNodeKey}, ToNodeKey={ToNodeKey}, Key={Key}";
        }
    }
}
