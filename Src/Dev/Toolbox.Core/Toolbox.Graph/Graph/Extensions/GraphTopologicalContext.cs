// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public class GraphTopologicalContext<TKey, TEdge>
        where TEdge : IGraphEdge<TKey>
    {
        private readonly HashSet<TKey> _processedNodeKeys;
        private readonly HashSet<TKey> _stopNodeKeys;
        private readonly object _lock = new object();

        public GraphTopologicalContext(IEqualityComparer<TKey>? equalityComparer = null)
        {
            equalityComparer = equalityComparer ??
                ((typeof(TKey) == typeof(string)) ? (IEqualityComparer<TKey>)StringComparer.OrdinalIgnoreCase : (IEqualityComparer<TKey>?)null);

            EdgeType = typeof(TEdge);
            _processedNodeKeys = new HashSet<TKey>(equalityComparer);
            _stopNodeKeys = new HashSet<TKey>(equalityComparer);
        }

        public GraphTopologicalContext(int maxLevels, IEqualityComparer<TKey>? equalityComparer = null)
            : this(equalityComparer)
        {
            maxLevels.VerifyAssert(x => x > 0, "Max levels must be greater then 0");

            MaxLevels = maxLevels;
        }

        public int? MaxLevels { get; }

        public Type EdgeType { get; }

        public IEnumerable<TKey> ProcessedNodeKeys
        {
            get
            {
                lock (_lock)
                {
                    return _processedNodeKeys;
                }
            }
        }

        public IEnumerable<TKey> StopNodeKeys
        {
            get
            {
                lock (_lock)
                {
                    return _stopNodeKeys;
                }
            }
        }

        public GraphTopologicalContext<TKey, TEdge> AddProcessedNodeKey(TKey value)
        {
            lock (_lock)
            {
                _processedNodeKeys.Add(value);
                _stopNodeKeys.Remove(value);
            }

            return this;
        }

        public GraphTopologicalContext<TKey, TEdge> AddStopNodeKey(TKey value)
        {
            lock (_lock)
            {
                _stopNodeKeys.Add(value);
                _processedNodeKeys.Remove(value);
            }

            return this;
        }
    }
}
