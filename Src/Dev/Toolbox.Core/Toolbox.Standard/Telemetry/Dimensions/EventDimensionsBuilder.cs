// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Build event dimension dictionary
    /// </summary>
    public class EventDimensionsBuilder
    {
        private readonly List<KeyValuePair<string, object>> _properties;

        public EventDimensionsBuilder()
        {
            _properties = new List<KeyValuePair<string, object>>();
        }

        public EventDimensionsBuilder(IEnumerable<KeyValuePair<string, object>> values)
        {
            _properties = values.ToList();
        }

        public EventDimensionsBuilder Add(string key, object? value)
        {
            _properties.Add(new KeyValuePair<string, object>(key, value!));
            return this;
        }

        public EventDimensionsBuilder Clear()
        {
            _properties.Clear();
            return this;
        }

        public IEventDimensions Build()
        {
            return new EventDimensions(_properties);
        }
    }
}
