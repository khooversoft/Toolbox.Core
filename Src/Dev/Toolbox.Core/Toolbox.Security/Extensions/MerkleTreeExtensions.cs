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
            hashes.Verify(nameof(hashes)).IsNotNull();

            return new MerkleTree()
                .Append(hashes.ToArray())
                .BuildTree().Value.ToArray()
                .Do(Convert.ToBase64String);
        }
    }
}
