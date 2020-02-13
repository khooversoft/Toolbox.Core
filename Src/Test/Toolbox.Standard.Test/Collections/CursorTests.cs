// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Collections
{
    public class CursorTests
    {
        [Fact]
        public void GivenCursor_EmptyList_ShouldPassTest()
        {
            var list = new List<int>();
            var cursor = new Cursor<int>(list);

            cursor.List.Count.Should().Be(0);
            cursor.Index.Should().Be(-1);
            cursor.Current.Should().Be(default);
            cursor.IsCursorAtEnd.Should().BeTrue();
            cursor.TryNext(out int value1).Should().BeFalse();
            cursor.TryPeek(out int value2).Should().BeFalse();
        }

        [Fact]
        public void GivenCursor_OneList_ShouldPassTest()
        {
            const int max = 1;
            var list = new List<int>(Enumerable.Range(0, max));
            var cursor = new Cursor<int>(list);

            cursor.List.Count.Should().Be(max);
            cursor.Index.Should().Be(-1);
            cursor.Current.Should().Be(default);
            cursor.IsCursorAtEnd.Should().BeTrue();
            cursor.TryPeek(out int value2).Should().BeTrue();

            cursor.TryNext(out int value1).Should().BeTrue();
            value1.Should().Be(0);
            cursor.Index.Should().Be(0);
            cursor.Current.Should().Be(0);
            cursor.IsCursorAtEnd.Should().BeFalse();

            cursor.TryNext(out int value3).Should().BeFalse();
            cursor.IsCursorAtEnd.Should().BeTrue();
        }

        [Fact]
        public void GivenCursor_TwoList_ShouldPassTest()
        {
            const int max = 10;
            var list = new List<int>(Enumerable.Range(0, max));
            var cursor = new Cursor<int>(list);

            cursor.List.Count.Should().Be(max);
            cursor.Index.Should().Be(-1);
            cursor.Current.Should().Be(default);
            cursor.IsCursorAtEnd.Should().BeTrue();

            int expectedValue = 0;
            while(cursor.TryNext(out int value))
            {
                value.Should().Be(expectedValue);

                cursor.Index.Should().Be(expectedValue);
                cursor.Current.Should().Be(expectedValue);

                expectedValue++;
            }

            cursor.IsCursorAtEnd.Should().BeTrue();

            cursor.Reset();
            expectedValue = 0;
            while (cursor.TryNext(out int value))
            {
                value.Should().Be(expectedValue);

                cursor.Index.Should().Be(expectedValue);
                cursor.Current.Should().Be(expectedValue);

                expectedValue++;
            }

            cursor.IsCursorAtEnd.Should().BeTrue();
        }
    }
}
