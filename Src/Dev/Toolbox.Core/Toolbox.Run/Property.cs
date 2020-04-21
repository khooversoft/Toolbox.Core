using Khooversoft.Toolbox.Standard;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Khooversoft.Toolbox.Run
{
    public class Property : IProperty
    {
        private readonly ConcurrentDictionary<string, object?> _properties = new ConcurrentDictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        public Property() => _properties = new ConcurrentDictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        public Property(IEnumerable<KeyValuePair<string, object?>> properties)
        {
            properties.VerifyNotNull(nameof(properties));

            _properties = new ConcurrentDictionary<string, object?>(properties, StringComparer.OrdinalIgnoreCase);
        }

        public void Clear() => _properties.Clear();

        public void Set<T>(T value) => Set<T>(typeof(T).Name, value);

        public void Set<T>(string name, T value) => _properties[name.VerifyNotEmpty(name)] = value;

        public T Get<T>() => Get<T>(typeof(T).Name);

        public T Get<T>(string name)
        {
            name.VerifyNotEmpty(nameof(name));

            if (!_properties.TryGetValue(name, out object? value)) throw new KeyNotFoundException($"{name} not found");

            return value!.ConvertToType<T>();
        }

        public bool TryGetValue<T>(out T value) => TryGetValue(typeof(T).Name, out value);

        public bool TryGetValue<T>(string name, out T value)
        {
            name.VerifyNotEmpty(nameof(name));
            value = default!;

            if (_properties.TryGetValue(name, out object? propertyValue))
            {
                value = propertyValue!.ConvertToType<T>();
                return true;
            }

            return false;
        }

        public bool Exist<T>() => _properties.ContainsKey(typeof(T).Name);

        public bool Exist(string name) => _properties.ContainsKey(name.VerifyNotEmpty(nameof(name)));

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _properties.GetEnumerator();
    }
}
