// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class ConvertToExtensions
    {
        private readonly static GuidConverter _guidConverter = new GuidConverter();

        /// <summary>
        /// Return list of public properties for object (anonymous)
        /// </summary>
        /// <typeparam name="T">type of self</typeparam>
        /// <param name="self">self</param>
        /// <returns>List of key value pairs</returns>
        public static IReadOnlyList<KeyValuePair<string, object>> ToKeyValues<T>(this T self)
        {
            self.VerifyNotNull(nameof(self));

            return self!.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name != "Item" && x.CanRead)
                .Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(self, null)))
                .ToList();
        }

        /// <summary>
        /// Convert value to type T
        /// </summary>
        /// <typeparam name="T">type to convert to</typeparam>
        /// <param name="valueToConvert">value to convert</param>
        /// <returns>converted type</returns>
        public static T ConvertToType<T>(this object valueToConvert)
        {
            if (valueToConvert == null) return default!;

            Type type = typeof(T);
            if (valueToConvert.GetType() == type) return (T)valueToConvert;

            var targetType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;

            if (targetType.IsEnum)
            {
                return (T)Enum.Parse(targetType, valueToConvert.ToString());
            }

            if (targetType == typeof(Guid) || targetType == typeof(Guid?))
            {
                return (T)_guidConverter.ConvertFrom(valueToConvert);
            }

            if (valueToConvert.GetType() == typeof(Guid) || valueToConvert.GetType() == typeof(Guid?))
            {
                return (T)_guidConverter.ConvertTo(valueToConvert, targetType);
            }

            return (T)Convert.ChangeType(valueToConvert, targetType);
        }

        /// <summary>
        /// Casts the specified object to the type specified through type.
        /// Has been introduced to allow casting objects without breaking the fluent API.
        /// <typeparam name="TTo"></typeparam>
        /// <param name="subject">subject to cast</param>
        public static T CastAs<T>(this object subject) where T : class
        {
            return (T)subject;
        }

        /// <summary>
        /// Try to casts the specified object to the type specified through type.
        /// Has been introduced to allow casting objects without breaking the fluent API.
        ///
        /// <typeparam name="TTo"></typeparam>
        /// <param name="subject">subject to cast</param>
        /// <param name="value">output value if same type, or default</param>
        public static bool TryCastAs<T>(this object subject, out T? value) where T : class
        {
            if(subject is T)
            {
                value = (T)subject;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Convert string to UTF8 bytes
        /// </summary>
        /// <param name="subject">string to convert</param>
        /// <returns>array of bytes</returns>
        public static IReadOnlyList<byte> ToBytes(this string subject)
        {
            if (subject == null) return Enumerable.Empty<byte>().ToList();

            return Encoding.UTF8.GetBytes(subject);
        }

        /// <summary>
        /// Convert string to UTF8 bytes
        /// </summary>
        /// <param name="subject">string to convert</param>
        /// <returns>array of bytes</returns>
        public static byte[] ToByteArray(this string subject)
        {
            if (subject == null) return new byte[0];

            return Encoding.UTF8.GetBytes(subject);
        }
    }
}
