using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Toolbox.BlockDocument
{
    internal static class Extensions
    {
        public static string ComputeSha256Hash(this byte[] inputBytes)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }
    }
}
