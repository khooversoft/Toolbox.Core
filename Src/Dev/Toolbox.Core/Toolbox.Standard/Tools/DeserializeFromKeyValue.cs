using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Deserialize from key value
    /// </summary>
    /// <typeparam name="T">type to create</typeparam>
    public class DeserializeFromKeyValue<T> where T : class, new()
    {
        public DeserializeFromKeyValue()
        {
        }

        public T ToObject(IEnumerable<KeyValuePair<string, object>> values)
        {
            values.Verify(nameof(values)).IsNotNull();

            var order = values
                .Select(x => new { x, Count = x.Key.Split(":", StringSplitOptions.RemoveEmptyEntries).Length })
                .OrderBy(x => x.Count)
                .ToList();

            T subject = (T)Activator.CreateInstance(typeof(T));

            foreach (var item in order)
            {
                SetPropertyValueByPath(subject, item.x.Key, item.x.Value);
            }

            return subject;
        }

        private void SetPropertyValueByPath(object objectToSet, string propertyPath, object valueToSet)
        {
            var propertyStack = propertyPath
                .Split(new char[] { ':' })
                .Reverse()
                .ToStack();

            while (propertyStack.TryPop(out string propertyName))
            {
                if (int.TryParse(propertyName, out int index))
                {
                    MethodInfo methodInfo = objectToSet.GetType().GetMethod("Add") ?? throw new ArgumentException("Cannot find 'Add' method");
                    methodInfo.Invoke(valueToSet, new object[] { valueToSet });
                    continue;
                }

                PropertyInfo pi = objectToSet.GetType().GetProperty(propertyName) ?? throw new ArgumentException($"Property {propertyName} not found on type {objectToSet.GetType().Name}");

                if (pi.PropertyType.IsClass && pi.PropertyType != typeof(string))
                {
                    object propertyObject = pi.GetValue(objectToSet);

                    if (propertyObject == null)
                    {
                        object newObject = Activator.CreateInstance(pi.PropertyType);
                        pi.SetValue(objectToSet, newObject);
                        objectToSet = newObject;
                        continue;
                    }

                    objectToSet = propertyObject;
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(pi.PropertyType) && pi.PropertyType != typeof(string))
                {
                    object propertyObject = pi.GetValue(objectToSet);

                    if (propertyObject == null)
                    {
                        Type constructedType = typeof(List<>).MakeGenericType(pi.PropertyType.GetGenericArguments());
                        object newObject = Activator.CreateInstance(constructedType);
                        pi.SetValue(objectToSet, newObject);
                        objectToSet = newObject;
                        continue;
                    }
                }

                Verify.Assert(propertyStack.Count == 0, $"{propertyPath} is invalid when scanning class type");
                objectToSet.SetPropertyValue(pi!.Name, valueToSet);
            }
        }
    }
}
