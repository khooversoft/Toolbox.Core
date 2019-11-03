// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Convert a scalar value to enumerable
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">object to convert</param>
        /// <returns>enumerator</returns>
        public static IEnumerable<T> ToEnumerable<T>(this T self)
        {
            yield return self;
        }

        /// <summary>
        /// Execute 'action' on each item
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="list">list to operate on</param>
        /// <param name="action">action to execute</param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// Convert enumerable to stack
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">enumerable</param>
        /// <returns>hash set</returns>
        public static Stack<T> ToStack<T>(this IEnumerable<T> self)
        {
            return new Stack<T>(self);
        }
    }
}
