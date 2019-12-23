// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Khooversoft.Toolbox.Security
{
    public interface IPrincipleSignatureContainer : IEnumerable<PrincipleSignature>
    {
        void Clear();

        PrincipleSignature? Get(string issuer);

        IPrincipleSignatureContainer Add(params PrincipleSignature[] principleSignature);
    }
}