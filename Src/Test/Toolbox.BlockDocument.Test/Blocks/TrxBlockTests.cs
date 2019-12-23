// Copyright (c) KhooverSoft. All rights reserved.
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
    public class TrxBlockTests
    {
        [Fact]
        public void GivenTrxrBlock_WhenInitialized_ShouldValidates()
        {
            var subject = new TrxBlock("referenceId", "credit", 100);
            subject.Digest.Should().NotBeNullOrEmpty();

            subject.Digest.Should().Be(subject.GetDigest());
        }

        [Fact]
        public void GivenTrxBlock_WhenSameInitialized_ShouldValidate()
        {
            var subject = new TrxBlock("referenceId", "credit", 100);
            subject.Digest.Should().NotBeNullOrEmpty();

            var s1 = new TrxBlock("referenceId", "credit", 100);
            s1.Digest.Should().NotBeNullOrEmpty();

            subject.Digest.Should().Be(s1.Digest);
        }

        [Fact]
        public void GivenTrxBlock_WhenCloned_ShouldValidate()
        {
            var subject = new TrxBlock("referenceId", "credit", 100);
            subject.Digest.Should().NotBeNullOrEmpty();
            subject.Digest.Should().Be(subject.GetDigest());

            var s1 = new TrxBlock(subject.ReferenceId, subject.TransactionType, subject.Value);
            subject.Digest.Should().Be(s1.GetDigest());
            (subject == s1).Should().BeTrue();
        }
    }
}
