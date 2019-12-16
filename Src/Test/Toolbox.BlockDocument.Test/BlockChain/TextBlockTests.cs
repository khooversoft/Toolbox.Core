// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.BlockDocument;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.BlockDocument.Test
{
    public class TextBlockTests
    {
        [Fact]
        public void GivenTextBlock_WhenConstructed_PropertiesShouldMatch()
        {
            TextBlock textBlock = new TextBlock("name", "contentType", "author", "content");

            textBlock.Should().NotBeNull();
            textBlock.Name.Should().Be("name");
            textBlock.ContentType.Should().Be("contentType");
            textBlock.Author.Should().Be("author");
            textBlock.Content.Should().Be("content");
        }

        [Fact]
        public void GivenTextBlock_WhenConstructedWithNulls_ShouldFail()
        {
            Action act = () => new TextBlock("", "contentType", "author", "content");
            act.Should().Throw<ArgumentException>();

            act = () => new TextBlock(null!, "contentType", "author", "content");
            act.Should().Throw<ArgumentException>();

            act = () => new TextBlock("name", "", "author", "content");
            act.Should().Throw<ArgumentException>();

            act = () => new TextBlock("name", "contentType", "", "content");
            act.Should().Throw<ArgumentException>();

            act = () => new TextBlock("name", "contentType", "author", null!);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenTextBlock_TestEqual_ShouldPass()
        {
            TextBlock v1 = new TextBlock("name", "contentType", "author", "content");

            TextBlock v2 = new TextBlock("name", "contentType", "author", "content");
            (v1 == v2).Should().BeTrue();
        }
    }
}
