// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Serialize class to key value list
    /// </summary>
    /// <typeparam name="T">type to serialize</typeparam>
    public class SerializeToKeyValue<T> where T : class
    {
        private readonly Stack<PropertyPath> _stack = new Stack<PropertyPath>();
        private readonly Func<string?, string, string> _createPath = (x, n) => (x != null ? x + ":" : string.Empty) + n;

        public SerializeToKeyValue()
        {
        }

        /// <summary>
        /// Return property path + value
        /// </summary>
        /// <param name="subject">object to scan</param>
        /// <param name="filter">filter for properties</param>
        /// <returns>list of property path values</returns>
        public IReadOnlyList<PropertyPathValue> ToKeyValue(T subject, Func<PropertyInfo, bool>? filter = null)
        {
            subject.VerifyNotNull(nameof(subject));

            _stack.Clear();
            _stack.Push(new PropertyPath(subject!, null));
            var propertyList = new List<PropertyPathValue>();
            filter ??= (x => true);

            while (_stack.Count > 0)
            {
                PropertyPath current = _stack.Pop();

                var classProperties = current.Instance.GetType().GetProperties()
                    .Where(x => filter(x))
                    .ToList();

                // Get value type and string
                classProperties
                    .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string))
                    .ForEach(x => propertyList.Add(new PropertyPathValue(_createPath(current.Path, x.Name), x.GetValue(current.Instance, null), x)));

                // Get class references
                classProperties
                    .Where(x => x.PropertyType.IsClass && x.PropertyType != typeof(string) && filter(x))
                    .Select(x => new { PropertyInfo = x, Value = x.GetValue(current.Instance, null) })
                    .Where(x => x.Value != null)
                    .Reverse()
                    .ForEach(x => _stack.Push(new PropertyPath(x.Value, _createPath(current.Path, x.PropertyInfo.Name))));

                var collection = classProperties
                    .Where(x => typeof(IEnumerable).IsAssignableFrom(x.PropertyType) && x.PropertyType != typeof(string))
                    .Select(x => new { PropertyInfo = x, Value = x.GetValue(current.Instance, null) })
                    .Where(x => x.Value != null)
                    .ToList();

                foreach (var collectionItem in collection)
                {
                    int index = -1;
                    foreach (var item in (IEnumerable)collectionItem.Value)
                    {
                        index++;

                        Type type = item.GetType();
                        if (type == typeof(string) || type.IsValueType)
                        {
                            propertyList.Add(new PropertyPathValue(_createPath(current.Path, $"{collectionItem.PropertyInfo.Name}:{index}"), item, collectionItem.PropertyInfo));
                            continue;
                        }

                        _stack.Push(new PropertyPath(item, _createPath(current.Path, $"{collectionItem.PropertyInfo.Name}:{index}")));
                    }
                }
            }

            return propertyList;
        }

        private struct PropertyPath
        {
            public PropertyPath(object instance, string? path)
            {
                Instance = instance;
                Path = path;
            }

            public object Instance { get; }

            public string? Path { get; }
        }
    }
}
