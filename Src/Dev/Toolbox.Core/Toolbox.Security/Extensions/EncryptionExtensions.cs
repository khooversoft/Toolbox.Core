// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Toolbox.Security
{
    public static class EncryptionExtensions
    {
        /// <summary>
        /// Encrypt with certificate (must be RSA)
        /// </summary>
        /// <param name="self">local certificate</param>
        /// <param name="context">work context</param>
        /// <param name="data">data to encrypted</param>
        /// <returns>byte array</returns>
        public static byte[] Encrypt(this X509Certificate2 self, byte[] data)
        {
            self.VerifyNotNull(nameof(self));
            data.VerifyNotNull(nameof(data));

            // GetRSAPublicKey returns an object with an independent lifetime, so it should be
            // handled via a using statement.
            using (RSA rsa = self.GetRSAPublicKey())
            {
                // OAEP allows for multiple hashing algorithms, what was formerly just "OAEP" is
                // now OAEP-SHA1.
                return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
            }
        }

        /// <summary>
        /// Decrypt data with certificate (must be RSA)
        /// </summary>
        /// <param name="self">local certificate</param>
        /// <param name="data">encrypted data</param>
        /// <returns>unencrypted byte array</returns>
        public static byte[] Decrypt(this X509Certificate2 self, byte[] data)
        {
            self.VerifyNotNull(nameof(self));
            data.VerifyNotNull(nameof(data));

            // GetRSAPrivateKey returns an object with an independent lifetime, so it should be
            // handled via a using statement.
            using (RSA rsa = self.GetRSAPrivateKey())
            {
                return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA1);
            }
        }
    }
}
