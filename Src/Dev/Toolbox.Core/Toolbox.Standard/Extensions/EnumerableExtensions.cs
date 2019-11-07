// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        /// <summary>
        /// Get head and tail of collection
        /// </summary>
        /// <typeparam name="T">type of collection</typeparam>
        /// <param name="self">collection</param>
        /// <param name="headCount">number of items to return from head</param>
        /// <param name="tailCount">number of items to return from tail</param>
        /// <returns></returns>
        public static IEnumerable<T> HeadAndTail<T>(this IEnumerable<T> self, int headCount, int tailCount)
        {
            if (self == null)
            {
                yield break;
            }

            var data = self.ToArray();
            int firstSize = Math.Min(headCount - 1, data.Length);
            int lastSize = data.Length - Math.Min(tailCount, data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                if (i <= firstSize || i >= lastSize)
                {
                    yield return data[i];
                }
            }
        }

        /// <summary>
        /// Shuffle list based on random crypto provider
        /// </summary>
        /// <typeparam name="T">type in list</typeparam>
        /// <param name="self">list to shuffle</param>
        /// <returns>shuffled list</returns>
        public static IReadOnlyList<T> Shuffle<T>(this IEnumerable<T> self)
        {
            self.Verify(nameof(self)).IsNotNull();

            var list = self.ToList();

            var provider = new RNGCryptoServiceProvider();
            int n = list.Count;

            while (n > 1)
            {
                var box = new byte[1];
                do
                {
                    provider.GetBytes(box);
                }
                while (!(box[0] < n * (Byte.MaxValue / n)));

                var k = (box[0] % n);
                n--;

                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
