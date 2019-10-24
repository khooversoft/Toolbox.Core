using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Standard
{
    public class StringTokenizer
    {
        private readonly List<ITokenSyntax> _syntaxList = new List<ITokenSyntax>();

        public StringTokenizer()
        {
        }

        public StringTokenizer UseCollapseWhitespace()
        {
            _syntaxList.Add(new WhiteSpaceSyntax());
            return this;
        }

        public StringTokenizer UseSingleQuote()
        {
            _syntaxList.Add(new BlockSyntax('\''));
            return this;
        }

        public StringTokenizer UseDoubleQuote()
        {
            _syntaxList.Add(new BlockSyntax('"'));
            return this;
        }

        public StringTokenizer Add(params string[] tokens)
        {
            _syntaxList.AddRange(tokens.Select(x => (ITokenSyntax)new TokenSyntax(x)));
            return this;
        }

        public StringTokenizer Add(params ITokenSyntax[] tokenSyntaxes)
        {
            _syntaxList.AddRange(tokenSyntaxes);
            return this;
        }

        public IReadOnlyList<IToken> Parse(params string[] sources)
        {
            return Parse(string.Join(string.Empty, sources));
        }

        public IReadOnlyList<IToken> Parse(string source)
        {
            ITokenSyntax[] syntaxRules = _syntaxList
                .OrderBy(x => x.Priority)
                .ToArray();

            var tokenList = new List<IToken>();
            int? dataStart = null;

            ReadOnlySpan<char> span = source.AsSpan();

            for(int index = 0; index < span.Length; index++)
            {
                int? matchLength = null;

                for(int syntaxIndex = 0; syntaxIndex < syntaxRules.Length; syntaxIndex++)
                {
                    matchLength = syntaxRules[syntaxIndex].Match(span.Slice(index));
                    if( matchLength == null)
                    {
                        continue;
                    }

                    if( dataStart != null)
                    {
                        string dataValue = span
                            .Slice((int)dataStart, index - (int)dataStart)
                            .ToString();

                        tokenList.Add(new TokenValue(dataValue));
                        dataStart = null;
                    }

                    tokenList.Add(syntaxRules[syntaxIndex].CreateToken(span.Slice(index, (int)matchLength)));
                    break;
                }

                if (matchLength == null)
                {
                    dataStart = dataStart ?? index;
                    continue;
                }

                index += (int)matchLength - 1;
            }

            if (dataStart != null)
            {
                string dataValue = span
                    .Slice((int)dataStart, span.Length - (int)dataStart)
                    .ToString();

                tokenList.Add(new TokenValue(dataValue));
            }

            return tokenList;
        }
    }
}
