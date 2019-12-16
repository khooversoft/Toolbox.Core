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
    public class StringVectorTests
    {
        [Theory]
        [InlineData(null, new string[0], false)]
        [InlineData("", new string[0], false)]
        [InlineData("first", new string[] { "first" }, false)]
        [InlineData(@"first\second", new string[] { @"first\second" }, false)]
        [InlineData("first/second", new string[] { "first", "second" }, false)]
        [InlineData("first/second/third", new string[] { "first", "second", "third" }, false)]
        [InlineData("/first/second/third", new string[] { "first", "second", "third" }, true)]
        public void StringVectorPrimaryTest(string value, string[] vectors, bool hasRoot)
        {
            StringVector sv = StringVector.Parse(value);

            sv.Count.Should().Be(vectors.Length);
            sv.Count.Should().Be(vectors.Length);
            sv.Where((x, i) => vectors[i] == x).Count().Should().Be(vectors.Length);

            sv.HasRoot.Should().Be(hasRoot);
            sv.ToString().Should().Be(value ?? string.Empty);
        }

        [Theory]
        [InlineData("first", "first")]
        [InlineData(@"first\second", @"first\second")]
        [InlineData("first/second", "first/second")]
        [InlineData("first/second/third", "first/second/third")]
        [InlineData("/first/second/third", "/first/second/third")]
        [InlineData("/first/../second/third", "/second/third")]
        [InlineData("/first/second/../third/fourth", "/first/third/fourth")]
        [InlineData("/first/second/./third/fourth", "/first/second/third/fourth")]
        [InlineData("/first/second/../../third/fourth", "/third/fourth")]
        public void GetAbsoultePathTest(string value, string expectedResult)
        {
            string result = value.GetAbsolutlePath();
            result.Should().Be(expectedResult);
            StringVector sv = StringVector.Parse(value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetAbsoultePathTest_RaiseException(string value)
        {
            Action act = () => value.GetAbsolutlePath();
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(@"first\second", "third", @"first\second/third")]
        [InlineData("first/second", "third", "first/second/third")]
        [InlineData("first/second/third", "fourth/fifth", "first/second/third/fourth/fifth")]
        [InlineData("/first/second/third", "fourth/fifth", "/first/second/third/fourth/fifth")]
        [InlineData("/first/../second/third", "fourth/fifth", "/second/third/fourth/fifth")]
        [InlineData("/first/second/../third/fourth", "fourth/fifth", "/first/third/fourth/fourth/fifth")]
        [InlineData("/first/second/../../third/fourth", "fifth/../six", "/third/fourth/six")]
        public void GetAbsoultePathTest2(string value, string combine, string expectedResult)
        {
            string result = value.GetAbsolutlePath(combine);
            result.Should().Be(expectedResult);
            StringVector sv = StringVector.Parse(value);
        }

        [Fact]
        public void GivenPath_WhenHasRoot_HasRootIsTrue()
        {
            string value = "/path/value";

            var sv = new StringVector(value, "/");
            sv.Should().NotBeNull();
            sv.HasRoot.Should().BeTrue();
        }

        [Fact]
        public void GivenPath_WhenDoesNotHaveRoot_HasRootIsFalse()
        {
            string value = "path/value";

            var sv = new StringVector(value, "/");
            sv.Should().NotBeNull();
            sv.HasRoot.Should().BeFalse();
        }
    }
}
