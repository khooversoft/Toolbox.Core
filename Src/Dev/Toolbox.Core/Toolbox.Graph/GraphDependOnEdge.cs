// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    [DebuggerDisplay("DependsOn: FromNodeKey={FromNodeKey}, ToNodeKey={ToNodeKey}, Key={Key}")]
    public class GraphDependOnEdge<TKey> : IGraphEdge<TKey>
    {
        public GraphDependOnEdge(TKey fromNodeKey, TKey toNodeKey)
        {
            Key = Guid.NewGuid();
            FromNodeKey = fromNodeKey;
            ToNodeKey = toNodeKey;
        }

        public Guid Key { get; }

        public TKey FromNodeKey { get; }

        public TKey ToNodeKey { get; }

        public override string ToString()
        {
            return $"{this.GetType().Name}, FromNodeKey={FromNodeKey}, ToNodeKey={ToNodeKey}, Key={Key}";
        }
    }
}
