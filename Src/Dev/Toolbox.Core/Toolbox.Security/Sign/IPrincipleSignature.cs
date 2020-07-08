// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;

namespace Khooversoft.Toolbox.Security
{
    public interface IPrincipleSignature
    {
        string Audience { get; }
        string Issuer { get; }
        RsaPublicPrivateKey PublicPrivateKey { get; }
        TimeSpan ValidFor { get; }

        string Sign(string payloadDigest);
        JwtTokenDetails? ValidateSignature(string jwt);
    }
}