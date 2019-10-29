// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public static class StringExtensions
    {
        /// <summary>
        /// Parse path
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static PathVector ParsePath(this string value, string delimiter = "/")
        {
            return new StringPathBuilder(delimiter)
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
    }
}
