﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Parser
{
    public class StringTokenizerTests
    {
        [Fact]
        public void BasicToken_WhenEmptyString_ShouldReturnNoTokens()
        {
            IReadOnlyList<IToken> tokens = new StringTokenizer()
                .UseCollapseWhitespace()
                .UseDoubleQuote()
                .UseSingleQuote()
                .Parse("");

            tokens.Count.Should().Be(0);
        }

        [Fact]
        public void BasicToken_WhenPadString_ShouldReturnSpaceToken()
        {
            IReadOnlyList<IToken> tokens = new StringTokenizer()
                .UseCollapseWhitespace()
                .UseDoubleQuote()
                .UseSingleQuote()
                .Parse("      ");

            tokens.Count.Should().Be(1);
            tokens[0].Value.Should().Be(" ");
        }

        [Fact]
        public void BasicToken_WhenTokenIsSpace_ShouldReturnValidTokens()
        {
            IReadOnlyList<IToken> tokens = new StringTokenizer()
                .UseCollapseWhitespace()
                .UseDoubleQuote()
                .UseSingleQuote()
                .Parse("abc def");

            var expectedTokens = new IToken[]
            {
                new TokenValue("abc"),
                new TokenValue(" "),
                new TokenValue("def"),
            };

            tokens.Count.Should().Be(expectedTokens.Length);

            tokens
                .Zip(expectedTokens, (o, i) => (o, i))
                .All(x => x.o.Value == x.i.Value)
                .Should().BeTrue();
        }

        [Fact]
        public void BasicToken_WhenTokenIsSpaceAndPad_ShouldReturnValidTokens()
        {
            IReadOnlyList<IToken> tokens = new StringTokenizer()
                .UseCollapseWhitespace()
                .UseDoubleQuote()
                .UseSingleQuote()
                .Parse("  abc   def  ");

            var expectedTokens = new IToken[]
            {
                new TokenValue(" "),
                new TokenValue("abc"),
                new TokenValue(" "),
                new TokenValue("def"),
                new TokenValue(" "),
            };

            tokens.Count.Should().Be(expectedTokens.Length);

            tokens
                .Zip(expectedTokens, (o, i) => (o, i))
                .All(x => x.o.Value == x.i.Value)
                .Should().BeTrue();
        }

        [Fact]
        public void BasicToken_WhenKnownTokenSpecified_ShouldReturnValidTokens()
        {
            IReadOnlyList<IToken> tokens = new StringTokenizer()
                .UseCollapseWhitespace()
                .UseDoubleQuote()
                .UseSingleQuote()
                .Add("[", "]")
                .Parse("  abc   [def]  ");

            var expectedTokens = new IToken[]
            {
                new TokenValue(" "),
                new TokenValue("abc"),
                new TokenValue(" "),
                new TokenValue("["),
                new TokenValue("def"),
                new TokenValue("]"),
                new TokenValue(" "),
            };

            tokens.Count.Should().Be(expectedTokens.Length);

            tokens
                .Zip(expectedTokens, (o, i) => (o, i))
                .All(x => x.o.Value == x.i.Value)
                .Should().BeTrue();
        }

        [Fact]
        public void PropertyName_WhenEscapeIsUsed_ShouldReturnValidTokens()
        {
            IReadOnlyList<IToken> tokens = new StringTokenizer()
                .Add("{", "}", "{{", "}}")
                .Parse("Escape {{firstName}} end");

            var expectedTokens = new IToken[]
            {
                new TokenValue("Escape "),
                new TokenValue("{{"),
                new TokenValue("firstName"),
                new TokenValue("}}"),
                new TokenValue(" end"),
            };

            tokens.Count.Should().Be(expectedTokens.Length);

            tokens
                .Zip(expectedTokens, (o, i) => (o, i))
                .All(x => x.o.Value == x.i.Value)
                .Should().BeTrue();
        }
    }
}
