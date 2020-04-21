// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Test if nullable type
        /// </summary>
        /// <param name="type">type to test</param>
        /// <returns>true if nullable, false if not</returns>
        public static bool IsNullableType(this Type type)
        {
            type.VerifyNotNull(nameof(type));

            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        /// <summary>
        /// Set property value based on name.  Handles null-able, enum, and guid along with base types
        /// </summary>
        /// <param name="objectToSet">base object</param>
        /// <param name="propertyName">property name</param>
        /// <param name="valueToSet">value to set</param>
        public static void SetPropertyValue(this object objectToSet, string propertyName, object? valueToSet)
        {
            objectToSet.VerifyNotNull(nameof(objectToSet));
            propertyName.VerifyNotEmpty(nameof(propertyName));

            Type type = objectToSet.GetType();

            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            propertyInfo = propertyInfo ?? throw new ArgumentException($"Property {propertyName} does not exist on object {objectToSet.GetType().Name}");

            if (valueToSet == null)
            {
                propertyInfo.SetValue(objectToSet, valueToSet, null);
                return;
            }

            var targetType = propertyInfo.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            if (targetType.IsEnum)
            {
                valueToSet = Enum.Parse(targetType, valueToSet.ToString());
                propertyInfo.SetValue(objectToSet, valueToSet, null);
                return;
            }

            if (propertyInfo.PropertyType == typeof(Guid) || propertyInfo.PropertyType == typeof(Guid?))
            {
                valueToSet = Guid.Parse(valueToSet.ToString());
                propertyInfo.SetValue(objectToSet, valueToSet, null);
                return;
            }

            if (propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.IsInterface)
            {
                propertyInfo.SetValue(objectToSet, valueToSet, null);
                return;
            }

            valueToSet = Convert.ChangeType(valueToSet, targetType);
            propertyInfo.SetValue(objectToSet, valueToSet, null);
        }

        /// <summary>
        /// Set value based on property path.  Property path is a string with ":" to separate property names.
        /// Will create objects for properties as property path is conversed
        /// </summary>
        /// <param name="objectToSet">base object to set</param>
        /// <param name="propertyPath">property path</param>
        /// <param name="valueToSet">value to set</param>
        public static void SetPropertyValueByPath(this object objectToSet, string propertyPath, object valueToSet)
        {
            var propertyStack = propertyPath
                .Split(new char[] { ':' })
                .Reverse()
                .ToStack();

            while (propertyStack.TryPop(out string propertyName))
            {
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

                Verify.Assert(propertyStack.Count == 0, $"{propertyPath} is invalid when scanning class type");
                objectToSet.SetPropertyValue(pi!.Name, valueToSet);
            }
        }

        /// <summary>
        /// Serialize to key value pairs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns>list of path and values</returns>
        public static IReadOnlyList<PropertyPathValue> SerializeToKeyValue<T>(this T value, Func<PropertyInfo, bool>? filter = null)
            where T : class
        {
            return new SerializeToKeyValue<T>().ToKeyValue(value, filter);
        }

        /// <summary>
        /// Get key value properties for a class and any sub classes based on an attribute
        /// </summary>
        /// <typeparam name="T">attribute type</typeparam>
        /// <param name="classToScan">class to scan</param>
        /// <returns>list of property path and values</returns>
        public static IReadOnlyList<PropertyPathValue> ToKeyValuesForAttribute<T>(this object classToScan)
            where T : Attribute
        {
            return classToScan.SerializeToKeyValue(x => x.GetCustomAttributes<T>().Count() > 0);
        }

        /// <summary>
        /// Convert object to cache object
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">value to cache</param>
        /// <param name="lifeTime">how long to cache object</param>
        /// <returns></returns>
        public static CacheObject<T> ToCacheObject<T>(this T? value, TimeSpan lifeTime)
            where T : class
        {
            return new CacheObject<T>(lifeTime)
                .Set(value!);
        }
    }
}
