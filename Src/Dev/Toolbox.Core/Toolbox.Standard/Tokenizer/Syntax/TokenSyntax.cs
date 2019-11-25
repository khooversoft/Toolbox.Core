// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public struct TokenSyntax : ITokenSyntax
    {
        public TokenSyntax(string token, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            Token = token;
            StringComparison = stringComparison;
            Priority = 3 + Token.Length;
        }

        public string Token { get; }

        public StringComparison StringComparison { get; }

        public int Priority { get; }

        public int? Match(ReadOnlySpan<char> span)
        {
            if (Token.Length > span.Length) return null;

            ReadOnlySpan<char> slice = span.Slice(0, Token.Length);
            if (Token.AsSpan().CompareTo(slice, StringComparison) == 0) return Token.Length;

            return null;
        }

        public IToken CreateToken(ReadOnlySpan<char> span)
        {
            string value = span.ToString();
            return new TokenValue(value);
        }
    }
}
