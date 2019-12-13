// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Provides the ability to take a set of properties that may have interpolate formats for
    /// property replacement.
    /// 
    /// Key = "value1", Value = "replace Value1"
    /// Key = "value2", Value = "{value1} changed"
    /// 
    /// "This {value1} is now" => "This replace Value1 is now"
    /// "This {value2} is now" => "This replace Value1 change is now"
    /// 
    /// The property can be recursive
    /// </summary>
    public class PropertyResolver : IPropertyResolver
    {
        private readonly IEqualityComparer<string> _comparer;

        public PropertyResolver(IEqualityComparer<string>? comparer = null, bool strict = false)
        {
            _comparer = comparer ?? StringComparer.OrdinalIgnoreCase;

            Strict = strict;
            SourceProperties = new Dictionary<string, string>(_comparer);
            Properties = Normalize();
        }

        public PropertyResolver(IReadOnlyDictionary<string, string> properties, IEqualityComparer<string>? comparer = null, bool strict = false)
        {
            properties.Verify(nameof(properties)).IsNotNull();

            _comparer = comparer ?? StringComparer.OrdinalIgnoreCase;

            Strict = strict;
            SourceProperties = properties.ToDictionary(x => x.Key, x => x.Value, _comparer);
            Properties = Normalize();
        }

        public PropertyResolver(IEnumerable<KeyValuePair<string, string>> properties, IEqualityComparer<string>? comparer = null, bool strict = false)
        {
            properties.Verify(nameof(properties)).IsNotNull();

            _comparer = comparer ?? StringComparer.OrdinalIgnoreCase;

            Strict = strict;
            SourceProperties = properties.ToDictionary(x => x.Key, x => x.Value, _comparer);
            Properties = Normalize();
        }

        /// <summary>
        /// Tokenizer for properties
        /// </summary>
        internal static readonly StringTokenizer Tokenizer = new StringTokenizer()
            .Add("{", "}", "{{", "}}");

        /// <summary>
        /// Strict mode
        /// </summary>
        public bool Strict { get; }

        /// <summary>
        /// Source properties, before normalization based on 
        /// </summary>
        public IReadOnlyDictionary<string, string> SourceProperties { get; }

        /// <summary>
        /// Resolved properties
        /// </summary>
        public IReadOnlyDictionary<string, string> Properties { get; }

        /// <summary>
        /// Resolve string based on properties
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>resolve value</returns>
        public string Resolve(string value)
        {
            return InternalResolve(value, Properties);
        }

        /// <summary>
        /// Create resolve that has strict set
        /// </summary>
        /// <param name="strict">if true, raise exception if property is not located in dictionary.</param>
        /// <returns>new instance of resolver with strict setting</returns>
        public IPropertyResolver WithStrict(bool strict = true)
        {
            return new PropertyResolver(Properties, _comparer, strict);
        }

        /// <summary>
        /// Create new resolver with new properties, use original key value pairs
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ignoreDuplicates">true, will not add keys already set</param>
        /// <returns>new property resolver</returns>
        public IPropertyResolver With(string key, string value, PropertyUpdate propertyUpdate)
        {
            key.Verify(nameof(key)).IsNotEmpty();
            value.Verify(nameof(value)).IsNotEmpty();

            return With(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(key, value) }, propertyUpdate);
        }

        /// <summary>
        /// Create new resolver with new properties, use original key value pairs
        /// </summary>
        /// <param name="values"></param>
        /// <param name="propertyUpdate">Update property mode</param>
        /// <returns>new property resolver</returns>
        public IPropertyResolver With(IEnumerable<KeyValuePair<string, string>> values, PropertyUpdate propertyUpdate)
        {
            values.Verify(nameof(values)).IsNotNull();

            if (!values.Any())
            {
                return this;
            }

            switch(propertyUpdate)
            {
                case PropertyUpdate.FailOnDuplicate:
                    return new PropertyResolver(SourceProperties.Concat(values), _comparer, Strict);

                case PropertyUpdate.IgnoreDuplicate:
                    IEnumerable<KeyValuePair<string, string>> newValues = SourceProperties.Concat(values.Where(x => !SourceProperties.ContainsKey(x.Key)));
                    return new PropertyResolver(newValues, _comparer, Strict);

                case PropertyUpdate.Overwrite:
                    var overwriteValues = values.Select(x => new { Index = 0, Value = x })
                        .Concat(SourceProperties.Select(x => new { Index = 1, Value = x }))
                        .GroupBy(x => x.Value.Key)
                        .Select(x => x.OrderBy(y => y.Index).Select(z => z.Value).First())
                        .ToList();

                    return new PropertyResolver(overwriteValues, _comparer, Strict);

                default:
                    throw new ArgumentException($"Invalided propertyUpdate={propertyUpdate}");
            }
        }

        /// <summary>
        /// Normalize property dictionary to be used later
        /// </summary>
        /// <returns>new property dictionary</returns>
        private IReadOnlyDictionary<string, string> Normalize()
        {
            var resolved = new Dictionary<string, string>(_comparer);

            foreach (var item in SourceProperties)
            {
                resolved[item.Key] = InternalResolve(item.Value, SourceProperties);
            }

            return resolved;
        }

        /// <summary>
        /// Resolve interpolated string using
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string resolved if possible</returns>
        private string InternalResolve(string value, IReadOnlyDictionary<string, string> properties)
        {
            if (value.IsEmpty())
            {
                return value;
            }

            IReadOnlyList<IToken> tokens = Tokenizer.Parse(value);
            var stack = new Stack<string>(tokens.Select(x => x.Value).Reverse());
            var str = new StringBuilder();

            while (stack.Count > 0)
            {
                string token = stack.Pop();

                switch (token)
                {
                    case "{{":
                    case "}}":
                        str.Append(token[0]);
                        break;

                    case "{":
                        Verify.Assert(stack.Count >= 2, $"Interpolate format error: {value}");
                        string propertyName = stack.Pop();
                        Verify.Assert(stack.Pop() == "}", $"Interpolate format error, missing ending '}}': {value}");

                        string propertyValue;
                        if (!properties.TryGetValue(propertyName, out propertyValue))
                        {
                            Verify.Assert(!Strict, $"Property {propertyName} cannot be found and is required");

                            str.Append("{").Append(propertyName).Append("}");
                            break;
                        }

                        str.Append(InternalResolve(propertyValue, properties));

                        break;

                    default:
                        str.Append(token);
                        break;
                }
            }

            return str.ToString();
        }
    }
}
