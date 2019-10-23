using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public struct TokenSyntaxQuotedString : ITokenSyntax
    {
        public IToken GetTokenValue(ReadOnlySpan<char> span)
        {
            int endingIndex = FindEndingQuote(span);
            string quotedString = span.Slice(1, endingIndex - 1).ToString();
            return new TokenValueQuoted(quotedString);
        }

        public bool Match(ReadOnlySpan<char> span)
        {
            if (span.Length == 0) return false;
            if (span[0] != '"' && span[0] != '\'') return false;

            FindEndingQuote(span);
            return true;
        }

        private int FindEndingQuote(ReadOnlySpan<char> span)
        {
            bool isEscape = false;
            for (int index = 1; index < span.Length; index++)
            {
                if (isEscape)
                {
                    isEscape = false;
                    if (span[index] == span[0]) continue;
                    throw new ArgumentException("Invalid escape sequence");
                }

                if (span[index] == span[0]) return index;
            }

            throw new ArgumentException("Missing ending quote");
        }
    }
}
