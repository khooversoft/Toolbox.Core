// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
            list.Verify(nameof(list)).IsNotNull();
            action.Verify(nameof(action)).IsNotNull();

            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// Execute 'action' on each item
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="list">list to operate on</param>
        /// <param name="action">action to execute</param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            list.Verify(nameof(list)).IsNotNull();
            action.Verify(nameof(action)).IsNotNull();

            int index = 0;
            foreach (var item in list)
            {
                action(item, index++);
            }
        }

        /// <summary>
        /// Execute 'action' on each item
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="list">list to operate on</param>
        /// <param name="action">action to execute</param>
        public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T, Task> action)
        {
            list.Verify(nameof(list)).IsNotNull();
            action.Verify(nameof(action)).IsNotNull();

            foreach (var item in list)
            {
                await action(item);
            }
        }

        /// <summary>
        /// Execute 'action' on each item
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="list">list to operate on</param>
        /// <param name="action">function to execute, index is passed</param>
        public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T, int, Task> action)
        {
            list.Verify(nameof(list)).IsNotNull();
            action.Verify(nameof(action)).IsNotNull();

            int index = 0;
            foreach (var item in list)
            {
                await action(item, index++);
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
            self.Verify(nameof(self)).IsNotNull();

            return new Stack<T>(self);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            tasks.Verify(nameof(tasks)).IsNotNull();

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            tasks.Verify(nameof(tasks)).IsNotNull();

            return Task.WhenAll(tasks);
        }
    }
}
