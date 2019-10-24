using System;

namespace Toolbox.Standard
{
    public interface ITokenSyntax
    {
        int Priority { get; }

        int? Match(ReadOnlySpan<char> span);

        IToken CreateToken(ReadOnlySpan<char> span);
    }
}