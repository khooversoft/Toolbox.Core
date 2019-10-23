using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public struct TokenSyntax<T> : ITokenSyntax where T : Enum
    {
        public TokenSyntax(string token, T tokenType)
        {
            Token = token;
            TokenType = tokenType;
        }

        public string Token { get; }

        public T TokenType { get; }

        public bool Match(ReadOnlySpan<char> span)
        {
            if (Token.Length > span.Length) return false;

            ReadOnlySpan<char> slice = span.Slice(0, Token.Length);
            if (Token.AsSpan().CompareTo(slice, StringComparison.OrdinalIgnoreCase) == 0) return true;

            return false;
        }

        public IToken GetTokenValue(ReadOnlySpan<char> span)
        {
            if (Token.Length > span.Length) throw new ArgumentException("Token length is greater then span length");

            ReadOnlySpan<char> slice = span.Slice(0, Token.Length);
            return new TokenValue<T>(slice.ToString(), TokenType);
        }
    }
}
