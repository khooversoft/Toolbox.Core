// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public abstract class NodeMapBase<TKey> : IGraphCommonMap<TKey>
    {
        private readonly List<IGraphCommonMap<TKey>> _list;

        protected NodeMapBase()
        {
            _list = new List<IGraphCommonMap<TKey>>();
        }

        public IReadOnlyList<IGraphCommonMap<TKey>> ChildNodes => _list;

        public void Add(IGraphCommonMap<TKey> node)
        {
            _list.Add(node);
        }

        public IEnumerator<IGraphCommonMap<TKey>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
