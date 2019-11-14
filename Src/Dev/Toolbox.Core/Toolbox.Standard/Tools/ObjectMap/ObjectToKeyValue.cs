using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class ObjectToKeyValue<T>
    {
        private readonly Stack<PropertyPath> _stack = new Stack<PropertyPath>();
        private readonly Func<string?, string, string> _createPath = (x, n) => (x != null ? x + ":" : string.Empty) + n;

        public ObjectToKeyValue()
        {
        }

        public IReadOnlyList<KeyValuePair<string, object>>? ToKeyValue(T subject, Func<PropertyInfo, bool>? filter = null)
        {
            if (subject == null) return null;

            _stack.Clear();
            _stack.Push(new PropertyPath(subject, null));
            var propertyList = new List<KeyValuePair<string, object>>();
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
                    .ForEach(x => propertyList.Add(new KeyValuePair<string, object>(_createPath(current.Path, x.Name), x.GetValue(current.Instance, null))));

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
                    int index = 0;
                    foreach (var item in (IEnumerable)collectionItem.Value)
                    {
                        _stack.Push(new PropertyPath(item, _createPath(current.Path, $"{collectionItem.PropertyInfo.Name}:{index}")));
                        index++;
                    }
                }

                //var values = collection
                //    .SelectMany(x => (IEnumerable)x.Value)
                //    .ToList();

                //collection
                //    .ForEach(x => _stack.Push(new PropertyPath(x.Value, _createPath(current.Path, $"{x.Index}:{x.PropertyInfo.Name}"))));
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
