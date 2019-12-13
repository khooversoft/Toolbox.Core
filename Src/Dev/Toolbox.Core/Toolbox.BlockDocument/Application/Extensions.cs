using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    internal static class Extensions
    {
        public static string ComputeSha256Hash(this IReadOnlyList<byte> inputBytes)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] outputBytes = sha256.ComputeHash(inputBytes.ToArray());

            return Convert.ToBase64String(outputBytes);
        }
    }
}
