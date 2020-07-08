// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Return types from assembly with that have attributes at the class level
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="attributes"></param>
        /// <returns>list of types that have attribute</returns>
        public static IEnumerable<Type> GetTypesWithAttribute(this Assembly assembly, params Type[] attributes)
        {
            assembly.VerifyNotNull(nameof(assembly));
            attributes.ForEach(x => x.VerifyAssert(y => y.GetType() == typeof(Attribute), y => $"{y.GetType()} is not a attribute type"));

            Func<Type, bool> testAttributes = x => x
                .GetCustomAttributes(true)
                .Any(x => attributes.Any(y => y.IsAssignableFrom(x.GetType())));

            return assembly.GetTypes()
                .Where(x => testAttributes(x))
                .ToList();
        }

        /// <summary>
        /// Return method infos that have a specific attribute.
        /// </summary>
        /// <typeparam name="T">attribute type</typeparam>
        /// <param name="subject">type to search</param>
        /// <returns>list of method infos and attribute</returns>
        public static IReadOnlyList<(MethodInfo MethodInfo, T Attribute)> GetMethodsWithAttribute<T>(this Type subject)
            where T : Attribute
        {
            return GetMethodsWithAttribute(subject, typeof(T))
                .Select(x => (MethodInfo: x.MethodInfo, Attribute: (T)x.Attributes.First()))
                .ToList();
        }

        /// <summary>
        /// Return method infos that have any specific attribute.
        /// </summary>
        /// <param name="subject">type to search</param>
        /// <param name="attributes">list of attribute to search for</param>
        /// <returns>list of method infos and attributes for any method that matches any of the attributes specified</returns>
        public static IReadOnlyList<(MethodInfo MethodInfo, object[] Attributes)> GetMethodsWithAttribute(this Type subject, params Type[] attributes)
        {
            subject.VerifyNotNull(nameof(subject));
            attributes.ForEach(x => x.VerifyAssert(y => y.GetType() == typeof(Attribute), y => $"{y.GetType()} is not a attribute type"));

            Func<MethodInfo, object[]> getRequiredAttributes = x => x.GetCustomAttributes(true)
                .Where(y => attributes.Any(z => z.GetType() == y.GetType()))
                .ToArray();

            var results = subject.GetMethods()
                .Select(x => (MethodInfo: x, Attributes: getRequiredAttributes(x)))
                .Where(x => x.Attributes.Length > 0)
                .ToList();

            return results;
        }
    }
}
