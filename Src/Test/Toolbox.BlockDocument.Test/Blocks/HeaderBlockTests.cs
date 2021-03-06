﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.BlockDocument;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.BlockDocument.Test.Blocks
{
    public class HeaderBlockTests
    {
        [Fact]
        public void GivenHeaderBlock_WhenInitialized_ShouldValidates()
        {
            var subject = new HeaderBlock("this is a description");
            subject.Digest.Should().NotBeNullOrEmpty();
            subject.Digest.Should().Be(subject.GetDigest());
        }

        [Fact]
        public void GivenHeaderBlock_WhenSameInitialized_ShouldValidate()
        {
            UnixDate now = UnixDate.UtcNow;

            var subject = new HeaderBlock(now, "Text-1111");
            subject.Digest.Should().NotBeNullOrEmpty();

            var s1 = new HeaderBlock(now, "Text-1111");
            s1.Digest.Should().NotBeNullOrEmpty();

            subject.Digest.Should().Be(s1.Digest);
        }

        [Fact]
        public void GivenHeaderBlock_WhenCloned_ShouldValidate()
        {
            UnixDate now = UnixDate.UtcNow;

            var subject = new HeaderBlock(now, "Text-2222");
            subject.Digest.Should().NotBeNullOrEmpty();

            var s1 = new HeaderBlock(subject.TimeStamp, subject.Description);
            subject.Digest.Should().Be(subject.GetDigest());
        }
    }
}
