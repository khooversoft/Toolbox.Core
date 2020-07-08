// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    [DebuggerDisplay("Key={Key}")]
    public class GraphNode<TKey> : IGraphNode<TKey>
    {
        public GraphNode(TKey key)
        {
            Key = key;
        }

        public TKey Key { get; }
    }
}
