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
        /// Convert enumerable to stack
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">enumerable</param>
        /// <returns>hash set</returns>
        public static Stack<T> ToStack<T>(this IEnumerable<T> self)
        {
            self.VerifyNotNull(nameof(self));

            return new Stack<T>(self);
        }

        /// <summary>
        /// Drain stack
        /// </summary>
        /// <typeparam name="T">T of stack</typeparam>
        /// <param name="subject">stack</param>
        /// <returns>enumerable of T</returns>
        public static IEnumerable<T> Drain<T>(this Stack<T> subject)
        {
            subject.VerifyNotNull(nameof(subject));

            while (subject.TryPop(out T item)) yield return item;
        }

        /// <summary>
        /// Drain stack
        /// </summary>
        /// <typeparam name="T">T of stack</typeparam>
        /// <param name="subject">stack</param>
        /// <returns>enumerable of T</returns>
        public static IEnumerable<T> Drain<T>(this Queue<T> subject)
        {
            subject.VerifyNotNull(nameof(subject));

            while (subject.TryDequeue(out T item)) yield return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tasks">task to join</param>
        /// <returns>task</returns>
        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            tasks.VerifyNotNull(nameof(tasks));

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="tasks">task to join</param>
        /// <returns>array of types</returns>
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            tasks.VerifyNotNull(nameof(tasks));

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Wait all
        /// </summary>
        /// <param name="tasks">task to wait on</param>
        public static void WaitAll(this IEnumerable<Task> tasks)
        {
            Task.WaitAll(tasks.ToArray());
        }
    }
}
