// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KHooversoft.Toolbox.Graph
{
    public class GraphFilter<TKey> : IGraphFilter<TKey>
    {
        private HashSet<TKey> _includeNodeKeys;
        private HashSet<TKey> _excludeNodeKeys;
        private readonly IEqualityComparer<TKey>? _keyCompare;

        public GraphFilter(IEqualityComparer<TKey>? equalityComparer = null)
        {
            _keyCompare = equalityComparer ??
                ((typeof(TKey) == typeof(string)) ? (IEqualityComparer<TKey>)StringComparer.OrdinalIgnoreCase : (IEqualityComparer<TKey>?)null);

            _includeNodeKeys = new HashSet<TKey>(_keyCompare);
            _excludeNodeKeys = new HashSet<TKey>(_keyCompare);
            IncludeLinkedNodes = true;
        }

        public IReadOnlyList<TKey> IncludeNodeKeys => _includeNodeKeys.ToList();

        public IReadOnlyList<TKey> ExcludeNodeKeys => _excludeNodeKeys.ToList();

        public bool IncludeLinkedNodes { get; private set; }

        public bool IncludeDependentNodes { get; private set; }

        public GraphFilter<TKey> Include(params TKey[] key)
        {
            key.ForEach(x => _includeNodeKeys.Add(x));
            return this;
        }

        public GraphFilter<TKey> Include(IEnumerable<TKey> keys)
        {
            keys?.ForEach(x => _includeNodeKeys.Add(x));
            return this;
        }

        public GraphFilter<TKey> Exclude(params TKey[] key)
        {
            key.ForEach(x => _excludeNodeKeys.Add(x));
            return this;
        }

        public GraphFilter<TKey> Exclude(IEnumerable<TKey> keys)
        {
            keys?.ForEach(x => _excludeNodeKeys.Add(x));
            return this;
        }

        public GraphFilter<TKey> SetIncludeLinkedNodes(bool state)
        {
            IncludeLinkedNodes = state;
            return this;
        }

        public GraphFilter<TKey> SetIncludeDependentNodes(bool state)
        {
            IncludeDependentNodes = state;
            return this;
        }
    }
}
