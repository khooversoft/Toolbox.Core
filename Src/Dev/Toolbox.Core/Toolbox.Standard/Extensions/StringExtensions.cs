﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class StringExtensions
    {
        /// <summary>
        /// Is string empty (null or white space)
        /// </summary>
        /// <param name="self">value</param>
        /// <returns>true or false</returns>
        public static bool IsEmpty(this string? self) => string.IsNullOrWhiteSpace(self);

        /// <summary>
        /// Convert string to null if null or whitespace
        /// </summary>
        /// <param name="subject">string</param>
        /// <returns>null or not empty string</returns>
        public static string? ToNullIfEmpty(this string? subject)
        {
            if (subject.IsEmpty()) return null;

            return subject;
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
            self.VerifyNotEmpty(nameof(self));

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
                    stack.Count.VerifyAssert<int, FormatException>(x => x > 0, _ => $"Relative path format error: {vectorValue}, cannot process '../'");

                    // Remove the last path vector
                    stack.Pop();
                    continue;
                }

                stack.Push(item);
            }

            return (vectorValue.HasRoot ? "/" : string.Empty) + string.Join("/", stack.Reverse());
        }

        /// <summary>
        /// Convert string to guid
        /// </summary>
        /// <param name="self">string to convert, empty string will return empty guid</param>
        /// <returns>guid or empty guid</returns>
        public static Guid ToGuid(this string self)
        {
            if (self.IsEmpty())
            {
                return Guid.Empty;
            }

            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(self));
                return new Guid(data);
            }
        }
    }
}
