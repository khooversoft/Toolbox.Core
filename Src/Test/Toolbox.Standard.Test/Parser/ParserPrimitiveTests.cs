// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Parser
{
    public class ParserPrimitiveTests
    {
        private enum SyntaxType
        {
            One,
            Two
        }

        [Fact]
        public void GivenTokenValue_WhenInitialize_PropertiesVerified()
        {
            const string testSubjectText = "TestSubject";
            var subject = new TokenValue<SyntaxType>(testSubjectText, SyntaxType.One);

            subject.Value.Should().Be(testSubjectText);
            subject.TokenType.Should().Be(SyntaxType.One);
        }

        [Fact]
        public void GivenTokenValueWithTypeNotDefault_WhenInitialize_PropertiesShouldVerify()
        {
            const string testSubjectText = "TestSubject123";
            var subject = new TokenValue<SyntaxType>(testSubjectText, SyntaxType.Two);

            subject.Value.Should().Be(testSubjectText);
            subject.TokenType.Should().Be(SyntaxType.Two);
        }

        [Fact]
        public void GivenTokenSyntax_WhenInitialize_PropertiesVerified()
        {
            const string testTokenValueText = "TokenText";
            var subject = new TokenSyntax<SyntaxType>(testTokenValueText, SyntaxType.One);

            subject.Token.Should().Be(testTokenValueText);
            subject.TokenType.Should().Be(SyntaxType.One);
        }

        [Fact]
        public void GivenTokenSyntaxWithTypeNotDefault_WhenInitialize_PropertiesShouldVerify()
        {
            const string testTokenValueText = "TokenText123";
            var subject = new TokenSyntax<SyntaxType>(testTokenValueText, SyntaxType.Two);

            subject.Token.Should().Be(testTokenValueText);
            subject.TokenType.Should().Be(SyntaxType.Two);
        }
    }
}
