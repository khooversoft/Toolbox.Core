// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class StringPathTests
    {
        [Theory]
        [InlineData("", new string[] { "" }, false)]
        [InlineData("/", new string[] { "" }, true)]
        [InlineData("/root", new string[] { "root" }, true)]
        [InlineData("root", new string[] { "root" }, false)]
        [InlineData("/root/second", new string[] { "root", "second" }, true)]
        [InlineData("/root/second/third", new string[] { "root", "second", "third" }, true)]
        public void TestPartPatterns(string value, string[] expectedParts, bool hasRoot)
        {
            StringVector subject = new StringVectorBuilder().Parse(value).Build();

            subject
                .Zip(expectedParts, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();

            subject.HasRoot.Should().Be(hasRoot);

            subject = value.ParsePath();

            subject
                .Zip(expectedParts, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();

            subject.HasRoot.Should().Be(hasRoot);
        }

        [Theory]
        [InlineData("", new string[] { "" }, true)]
        [InlineData("/", new string[] { "" }, false)]
        [InlineData("/root", new string[] { "root" }, false)]
        [InlineData("root", new string[] { "root" }, true)]
        [InlineData("/root/second", new string[] { "root", "second" }, false)]
        [InlineData("/root/second/third", new string[] { "root", "second", "third" }, false)]
        public void TestPartPatternsFailures(string value, string[] expectedParts, bool hasRoot)
        {
            StringVector subject = new StringVectorBuilder().Parse(value).Build();

            subject
                .Zip(expectedParts, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();

            subject.HasRoot.Should().Be(!hasRoot);
        }

        [Theory]
        [InlineData(false, new string[] { }, "")]
        [InlineData(false, new string[] { "first" }, "first")]
        [InlineData(false, new string[] { "first", "second" }, "first/second")]
        [InlineData(true, new string[] { }, "/")]
        [InlineData(true, new string[] { "first" }, "/first")]
        [InlineData(true, new string[] { "first", "second" }, "/first/second")]
        public void TestStringPathBuilder_WhenBuilt_ShouldNotFail(bool hasRoot, string[] parts, string expected)
        {
            StringVector path = new StringVectorBuilder()
                .SetHasRoot(hasRoot)
                .Add(parts)
                .Build();

            path.ToString().Should().Be(expected);
            expected.Should().Be(path);
        }
    }
}
