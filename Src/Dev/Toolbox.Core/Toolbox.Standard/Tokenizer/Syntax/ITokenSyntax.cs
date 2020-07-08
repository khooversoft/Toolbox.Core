// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// General token syntax interface for processing different types of tokens.
    /// 
    /// This follows the "test" then "do" pattern.  Match test to see if the token has been encountered
    /// while "CreateToken" returns the token from the span.
    /// </summary>
    public interface ITokenSyntax
    {
        int Priority { get; }

        int? Match(ReadOnlySpan<char> span);

        IToken CreateToken(ReadOnlySpan<char> span);
    }
}