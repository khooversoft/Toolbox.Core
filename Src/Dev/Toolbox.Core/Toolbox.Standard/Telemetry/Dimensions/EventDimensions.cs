// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class EventDimensions : IEventDimensions
    {
        private Dictionary<string, object> _dimension;

        public EventDimensions()
        {
            _dimension = new Dictionary<string, object>();
        }

        public EventDimensions(IEnumerable<KeyValuePair<string, object>> values)
        {
            values.Verify(nameof(values)).IsNotNull();

            _dimension = values.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        public static IEventDimensions Empty { get; } = new EventDimensions();

        public object this[string key] => _dimension[key];

        public IEnumerable<string> Keys => _dimension.Keys;

        public IEnumerable<object> Values => _dimension.Values;

        public int Count => _dimension.Count;

        public bool ContainsKey(string key) => _dimension.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dimension.GetEnumerator();

        public bool TryGetValue(string key, out object value) => _dimension.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _dimension.GetEnumerator();

        public static EventDimensions operator +(EventDimensions self, IEventDimensions right)
        {
            self.Verify(nameof(self)).IsNotNull();
            right.Verify(nameof(right)).IsNotNull();

            var newDimensions = new Dictionary<string, object>();

            self.ForEach(x => newDimensions[x.Key] = x.Value);
            right.ForEach(x => newDimensions[x.Key] = x.Value);

            return new EventDimensions(newDimensions);
        }
    }
}
