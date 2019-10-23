using System;

namespace Toolbox.Standard
{
    public interface ITokenSyntax
    {
        bool Match(ReadOnlySpan<char> span);

        public IToken GetTokenValue(ReadOnlySpan<char> span);
    }
}