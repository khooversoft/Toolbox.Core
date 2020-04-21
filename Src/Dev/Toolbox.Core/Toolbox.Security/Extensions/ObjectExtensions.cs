// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.Security
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Convert bytes to SHA256 hash
        /// </summary>
        /// <param name="inputBytes">input bytes</param>
        /// <returns>hash as base 64</returns>
        public static string ToSHA256Hash(this IEnumerable<byte> inputBytes)
        {
            inputBytes.VerifyNotNull(nameof(inputBytes));

            return SHA256.Create()
                .ComputeHash(inputBytes.ToArray())
                .Func(Convert.ToBase64String);
        }
    }
}
