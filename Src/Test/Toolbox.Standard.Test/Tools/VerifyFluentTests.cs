using FluentAssertions;
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
            string root = null;

            VerifyContext<string> context = root.Verify(nameof(root));

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

            const string msg = "length root must be greater then 5";
            Action act = () => root.Verify(nameof(root)).IsNotEmpty().Assert(x => x.Length > 5, msg);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenVerifyOnString_WhenNull_ShouldThrow()
        {
            string root = null;

            const string msg = "length root must be greater then 5";
            Action act = () => root.Verify(nameof(root)).IsNotEmpty().Assert(x => x.Length > 5, msg);

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
    }
}
