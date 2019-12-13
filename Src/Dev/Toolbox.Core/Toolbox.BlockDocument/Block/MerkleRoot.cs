using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class MerkleRoot
    {
        public string Build(IEnumerable<string> leaves)
        {
            if (leaves == null || !leaves.Any()) return string.Empty;

            var merkelLeaves = new List<string>(leaves);
            if (merkelLeaves.Count == 1) return merkelLeaves[0];

            if (merkelLeaves.Count % 2 > 0) merkelLeaves.Add(merkelLeaves.Last());

            var merkelBranches = new List<string>();

            for(int i = 0; i < merkelLeaves.Count; i+= 2)
            {
                var leafPair = string.Concat(merkelLeaves[i], merkelLeaves[i + 1]);

                merkelBranches.Add(HashUsingSHA256(HashUsingSHA256(leafPair)));
            }

            return Build(merkelBranches);
        }

        private static string HashUsingSHA256(string data)
        {
            using (var sha256 = SHA256Managed.Create())
            {
                return ComputeHash(sha256, HexToByteArray(data));
            }
        }

        private static string ComputeHash(HashAlgorithm hashAlgorithm, byte[] input)
        {
            byte[] data = hashAlgorithm.ComputeHash(input);
            return ByteArrayToHex(data);
        }

        private static string ByteArrayToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        private static byte[] HexToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
