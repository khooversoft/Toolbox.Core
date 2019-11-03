// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class VerifyFluentTests
    {
        [Fact]
        public void GivenVerifyOnString_WhenBinded_ShouldReturnVerifyContext()
        {
            string root = "This is a test";

            VerifyContext<string> context = root.Verify(nameof(root));

            context.Should().NotBeNull();
            context.Name.Should().Be(nameof(root));
            context.Value.Should().Be(root);
        }

        [Fact]
        public void GivenVerifyOnNullString_WhenBinded_ShouldReturnVerifyContext()
        {
            string? root = null;

            VerifyContext<string?> context = root.Verify(nameof(root));

            context.Should().NotBeNull();
            context.Name.Should().Be(nameof(root));
            context.Value.Should().BeNull();
        }

        [Fact]
        public void GivenVerifyOnInt_WhenBinded_ShouldReturnVerifyContext()
        {
            int intRoot = 10;

            VerifyContext<int> context = intRoot.Verify(nameof(intRoot));

            context.Should().NotBeNull();
            context.Name.Should().Be(nameof(intRoot));
            context.Value.Should().Be(intRoot);
        }

        [Fact]
        public void GivenVerifyOnIntNullable_WhenWithValue_ShouldReturnVerifyContext()
        {
            int? intRoot = 10;

            VerifyContext<int?> context = intRoot.Verify(nameof(intRoot));

            context.Should().NotBeNull();
            context.Name.Should().Be(nameof(intRoot));
            context.Value.Should().Be(intRoot);
        }

        [Fact]
        public void GivenVerifyOnIntNullable_WhenNull_ShouldReturnVerifyContext()
        {
            int? intRoot = null;

            VerifyContext<int?> context = intRoot.Verify(nameof(intRoot));

            context.Should().NotBeNull();
            context.Name.Should().Be(nameof(intRoot));
            context.Value.Should().Be(intRoot);
        }

        [Fact]
        public void GivenVerifyOnString_WhenSpacePadded_ShouldThrow()
        {
            string root = "   ";

            Action act = () => root.Verify(nameof(root)).IsNotEmpty().Assert(x => x.Length > 5, "length root must be greater then 5");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenVerifyOnInt_WhenValueIsValid_ShouldNotThrow()
        {
            int value = 10;

            Action act = () => value.Verify().Assert(x => x > 5, "message");

            act.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void GivenVerifyOnInt_WhenValueIsNotValid_ShouldThrow()
        {
            int value = 10;

            Action act = () => value.Verify().Assert(x => x > 10, "message");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenVerifyOnInt_WhenValueIsNotValid_ShouldThrowCustom()
        {
            int value = 10;

            Action act = () => value.Verify().Assert<int, FormatException>(x => x > 10, "message");

            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void GivenVerifyOnString_WhenNull_ShouldThrow()
        {
            string? root = null;

            const string msg = "length root must be greater then 5";
            Action act = () => root!.Verify(nameof(root)).IsNotEmpty().Assert(x => x.Length > 5, msg);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenVerifyOnString_WhenGreaterThenRequired_ShouldNotThrow()
        {
            string root = "this is a test";

            const string msg = "length root must be greater then 5";
            Action act = () => root.Verify(nameof(root)).IsNotEmpty().Assert(x => x.Length > 5, msg);

            act.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void GivenVerifyOnString_WhenLessThenRequired_ShouldThrow()
        {
            string root = "this";

            const string msg = "length root must be greater then 5";
            Action act = () => root.Verify(nameof(root)).Assert(x => !x.IsEmpty() && x.Length > 5, msg);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenVerifyIsNotNull_WhenNull_ShouldThrow()
        {
            string? root = null;

            Action act = () => root!.Verify(nameof(root)).IsNotEmpty();

            act.Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenVerifyIsEmpty_WhenEmpty_ShouldThrow()
        {
            string? root = "";

            Action act = () => root.Verify(nameof(root)).IsNotEmpty();

            act.Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenVerifyIsEmpty_WhenNotEmpty_ShouldNotThrow()
        {
            string? root = "data";

            Action act = () => root.Verify(nameof(root)).IsNotEmpty();

            act.Should()
                .NotThrow();
        }
    }
}
