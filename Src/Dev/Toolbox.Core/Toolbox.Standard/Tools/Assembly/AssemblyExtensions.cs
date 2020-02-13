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
        public static IEnumerable<Type> GetTypesWithAttribute(this Assembly assembly, params Type[] attributes)
        {
            assembly.Verify(nameof(assembly)).IsNotNull();
            attributes.ForEach(x => x.Verify().Assert(y => y.GetType() == typeof(Attribute), y => $"{y.GetType()} is not a attribute type"));

            Func<Type, bool> testAttributes = x => x.GetCustomAttributes(true)
                .Any(x => attributes.Any(y => y.IsAssignableFrom(x.GetType())));

            return assembly.GetTypes()
                .Where(x => testAttributes(x))
                .ToList();
        }

        public static IReadOnlyList<(MethodInfo MethodInfo, T Attribute)> GetMethodsWithAttribute<T>(this Type subject)
        {
            return GetMethodsWithAttribute(subject, typeof(T))
                .Select(x => (MethodInfo: x.MethodInfo, Attribute: (T)x.Attributes.First()))
                .ToList();
        }

        public static IReadOnlyList<(MethodInfo MethodInfo, object[] Attributes)> GetMethodsWithAttribute(this Type subject, params Type[] attributes)
        {
            subject.Verify(nameof(subject)).IsNotNull();
            attributes.ForEach(x => x.Verify().Assert(y => y.GetType() == typeof(Attribute), y => $"{y.GetType()} is not a attribute type"));

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
