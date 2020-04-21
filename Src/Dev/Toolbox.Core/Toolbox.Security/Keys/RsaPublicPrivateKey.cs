// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.Security
{
    public class RsaPublicPrivateKey
    {
        public RsaPublicPrivateKey(Guid? kid = null)
        {
            Kid = kid ?? Guid.NewGuid();

            using var rsa = RSA.Create();
            PublicKey = rsa.ExportParameters(includePrivateParameters: false);
            PrivateKey = rsa.ExportParameters(includePrivateParameters: true);
        }

        public RsaPublicPrivateKey(Guid kid, RSAParameters? publicKey = null, RSAParameters? privateKey = null)
        {
            Kid = kid;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public Guid Kid { get; }

        public RSAParameters? PublicKey { get; }

        public RSAParameters? PrivateKey { get; }

        public RSAParameters GetPublicKey() => PublicKey ?? throw new ArgumentException("PublicKey required");

        public RSAParameters GetPrivateKey() => PrivateKey ?? throw new ArgumentException("PrivateKey required");
    }
}
