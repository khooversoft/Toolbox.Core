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
        public static string? Resolve(this string? self, IPropertyResolver resolver)
        {
            if (self.IsEmpty())
            {
                return self;
            }

            resolver.Verify(nameof(resolver)).IsNotNull();
            return resolver.Resolve(self!);
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
        public static IPropertyResolver BuildResolver(this object classToScan, params IEnumerable<KeyValuePair<string, string>>[] properties)
        {
            var classProperties = classToScan.ToKeyValuesForAttribute<PropertyResolverAttribute>()
                .Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString()!));

            var result = properties
                .SelectMany(x => x)
                .Concat(classProperties)
                .ToList();

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            result.ForEach(x => dict[x.Key] = x.Value.Trim());

            return new PropertyResolver(dict);
        }
    }
}
