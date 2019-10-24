using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public struct TokenSyntax<T> : ITokenSyntax where T : Enum
    {
        public TokenSyntax(string token, T tokenType, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            Token = token;
            TokenType = tokenType;
            StringComparison = stringComparison;
            Priority = 3 + Token.Length;
        }

        public string Token { get; }

        public T TokenType { get; }

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
            return new TokenValue<T>(value, TokenType);
        }
    }
}
