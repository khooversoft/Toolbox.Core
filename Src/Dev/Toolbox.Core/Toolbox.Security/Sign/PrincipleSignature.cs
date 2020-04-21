// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Security
{
    public class PrincipleSignature : IPrincipleSignature
    {
        public PrincipleSignature(string issuer, string audience, TimeSpan validFor, RsaPublicPrivateKey publicPrivateKey)
        {
            issuer.VerifyNotEmpty(nameof(issuer));
            publicPrivateKey.VerifyNotNull(nameof(publicPrivateKey));

            Issuer = issuer;
            Audience = audience;
            ValidFor = validFor;
            PublicPrivateKey = publicPrivateKey;
        }

        public string Issuer { get; }

        public string Audience { get; }

        public TimeSpan ValidFor { get; }

        public RsaPublicPrivateKey PublicPrivateKey { get; }

        public string Sign(string payloadDigest)
        {
            return new JwtTokenBuilder()
                .SetDigest(payloadDigest)
                .SetIssuer(Issuer)
                .SetAudience(Audience)
                .SetExpires(DateTime.UtcNow.Add(ValidFor))
                .SetPrivateKey(PublicPrivateKey)
                .Build();
        }

        public JwtTokenDetails? ValidateSignature(IWorkContext context, string jwt)
        {
            return new JwtTokenParser(PublicPrivateKey.ToEnumerable(), Issuer.ToEnumerable(), Audience.ToEnumerable())
                .Parse(context, jwt);
        }
    }
}
