﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class PropertyResolverExtensions
    {
        /// <summary>
        /// Resolve property
        /// </summary>
        /// <param name="self">string to resolve</param>
        /// <param name="resolver">resolver</param>
        /// <returns>resolved string</returns>
        public static string? Resolve(this string self, IPropertyResolver resolver)
        {
            if (self.IsEmpty())
            {
                return self ?? string.Empty;
            }

            resolver.VerifyNotNull(nameof(resolver));
            return resolver.Resolve(self);
        }

        /// <summary>
        /// Get property names in string
        /// </summary>
        /// <param name="self">string to scan</param>
        /// <returns>list of properties</returns>
        public static IReadOnlyList<string>? GetPropertyNames(this string self)
        {
            if (self.IsEmpty())
            {
                return null;
            }

            IReadOnlyList<IToken> tokens = PropertyResolver.Tokenizer.Parse(self);
            var stack = new Stack<string>(tokens.Select(x => x.Value).Reverse());
            var list = new List<string>();

            while (stack.Count > 0)
            {
                string token = stack.Pop();

                switch (token)
                {
                    case "{":
                        Verify.Assert(stack.Count >= 2, $"Interpolate format error: {self}");
                        string propertyName = stack.Pop();
                        Verify.Assert(stack.Pop() == "}", $"Interpolate format error, missing ending '}}': {self}");

                        list.Add(propertyName);
                        break;

                    default:
                        break;
                }
            }

            return list;
        }

        /// <summary>
        /// Build property resolver for properties with PropertyResolverAttribute and properties specified.  Keys are merged
        /// giving the class properties high precedent, then properties specified in order
        /// </summary>
        /// <param name="classToScan">class to scan for PropertyResolveAttribute</param>
        /// <param name="properties">Additional properties (has precedent)</param>
        /// <returns>property resolver</returns>
        public static IPropertyResolver BuildResolver<T>(this T classToScan, params IEnumerable<KeyValuePair<string, string>>[] properties)
             where T : class
        {
            var classProperties = classToScan.ToKeyValuesForAttribute<PropertyResolverAttribute>()
                .Select(x => {
                    PropertyResolverAttribute attr = x.PropertyInfo.GetCustomAttribute<PropertyResolverAttribute>();
                    if (attr == null || attr.PropertyName.IsEmpty()) return x;

                    string path = x.Path.Split("/")[0..^1]
                        .Concat(attr.PropertyName.ToEnumerable())
                        .Func(x => string.Join("/", x));

                    return new PropertyPathValue(path, x.Value, x.PropertyInfo);
                })
                .Select(x => new KeyValuePair<string, string>(x.Path, x.Value?.ToString()!));

            var result = properties
                .SelectMany(x => x)
                .Concat(classProperties)
                .ToList();

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            result.ForEach(x => dict[x.Key] = x.Value.Trim());

            return new PropertyResolver(dict);
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
    }
}
