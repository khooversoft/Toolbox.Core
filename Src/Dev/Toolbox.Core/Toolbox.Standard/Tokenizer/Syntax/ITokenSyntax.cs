// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Toolbox.Standard
{
    public interface ITokenSyntax
    {
        int Priority { get; }

        int? Match(ReadOnlySpan<char> span);

        IToken CreateToken(ReadOnlySpan<char> span);
    }
}