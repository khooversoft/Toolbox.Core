// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public struct BlockSyntax : ITokenSyntax
    {
        public BlockSyntax(char blockSignal = '"')
        {
            BlockSignal = blockSignal;
            Priority = 1;
        }

        public char BlockSignal { get; }

        public int Priority { get; }

        public int? Match(ReadOnlySpan<char> span)
        {
            if (span.Length == 0) return null;
            if (span[0] != BlockSignal) return null;

            bool isEscape = false;
            for (int index = 1; index < span.Length; index++)
            {
                if (isEscape)
                {
                    isEscape = false;
                    if (span[index] != BlockSignal) throw new ArgumentException("Invalid escape sequence");
                    continue;
                }

                if (span[index] == '\\')
                {
                    isEscape = true;
                    continue;
                }

                if (span[index] == BlockSignal) return index;
            }

            throw new ArgumentException("Missing ending quote");
        }

        public IToken CreateToken(ReadOnlySpan<char> span)
        {
            string value = span.ToString();
            return new BlockToken(value);
        }
    }
}
