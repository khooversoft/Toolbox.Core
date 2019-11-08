// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class StringExtensions
    {
        /// <summary>
        /// Parse path
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static StringVector ParsePath(this string value, string delimiter = "/")
        {
            return new StringVectorBuilder(delimiter)
                .Parse(value)
                .Build();
        }

        /// <summary>
        /// Is string empty (null or white space)
        /// </summary>
        /// <param name="self">value</param>
        /// <returns>true or false</returns>
        public static bool IsEmpty(this string? self)
        {
            return string.IsNullOrWhiteSpace(self);
        }

        /// <summary>
        /// The equivalent to Path.GetFullPath which returns absolute but for vectors (including URI's)
        /// 
        /// Resolves the 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="values"></param>
        /// <returns>return absolution path</returns>
        public static string GetAbsolutlePath(this string self, params string[] values)
        {
            self.Verify(nameof(self)).IsNotEmpty();

            StringVector vectorValue = StringVector.Parse(self.Trim())
                .With(values);

            var vectors = vectorValue
                .Where(x => !x.IsEmpty() && x != ".")
                .ToList();

            var stack = new Stack<string>();

            foreach (var item in vectors)
            {
                if (item == "..")
                {
                    stack.Count.Verify().Assert<int, FormatException>(x => x > 0, $"Relative path format error: {vectorValue}, cannot process '../'");

                    // Remove the last path vector
                    stack.Pop();
                    continue;
                }

                stack.Push(item);
            }

            return (vectorValue.HasRoot ? "/" : string.Empty) + string.Join("/", stack.Reverse());
        }
    }
}
