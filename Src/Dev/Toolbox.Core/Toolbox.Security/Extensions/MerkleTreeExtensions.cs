// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Security
{
    public static class MerkleTreeExtensions
    {
        public static string ToMerkleHash(this IEnumerable<string> hashes)
        {
            hashes.VerifyNotNull(nameof(hashes));

            return new MerkleTree()
                .Append(hashes.ToArray())
                .BuildTree().Value.ToArray()
                .Func(Convert.ToBase64String);
        }
    }
}
