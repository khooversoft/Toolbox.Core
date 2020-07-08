// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    [Trait("Category", "Toolbox")]
    public class DeferredTests
    {
        [Fact]
        public void GivenDeferred_WhenSetWithLambda_ShouldPass()
        {
            const string testText = "test";

            var subject = new Deferred<string>(() => testText);

            subject.Value.Should().Be(testText);
            subject.Value.Should().Be(testText);
        }

        [Fact]
        public void GivenDeferred_WhenThrow_ShouldThrow()
        {
            var subject = new Deferred<string>(() => throw new ArgumentException());

            subject.Invoking(x => x.Value).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenDeferred_WhenActionSet_ShouldPerformAction()
        {
            int value = 0;

            var subject = new Deferred(() => value++);

            subject.Execute();
            value.Should().Be(1);

            subject.Execute();
            value.Should().Be(1);
        }

        [Fact]
        public void GivenDeferred_WhenActionThrow_ShouldThrow()
        {
            var subject = new Deferred(() => throw new ArgumentException());

            subject.Invoking(x => x.Execute()).Should().Throw<ArgumentException>();
        }
    }
}
