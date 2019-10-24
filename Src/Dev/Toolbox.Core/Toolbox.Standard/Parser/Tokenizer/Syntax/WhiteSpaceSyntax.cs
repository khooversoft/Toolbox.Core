using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public struct WhiteSpaceSyntax : ITokenSyntax
    {
        public int Priority { get; }

        public IToken CreateToken(ReadOnlySpan<char> span)
        {
            return new TokenValue(" ");
        }

        public int? Match(ReadOnlySpan<char> span)
        {
            for(int i = 0; i < span.Length; i++)
            {
                if( char.IsWhiteSpace(span[i]))
                {
                    continue;
                }

                return i > 0 ? i : (int?)null;
            }

            return span.Length;
        }
    }
}
