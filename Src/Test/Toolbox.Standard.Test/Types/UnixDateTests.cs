// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Types
{
    public class UnixDateTests
    {
        [Fact]
        public void GivenDate_WhenUsedWithType_ShouldPass()
        {
            UnixDate d1 = UnixDate.UtcNow;
            UnixDate d2 = d1;
            d2.TimeStamp.Should().Be(d1.TimeStamp);

            long v1 = d1.TimeStamp;
            v1.Should().Be(d2.TimeStamp);

            UnixDate d3 = (UnixDate)v1;
            d3.TimeStamp.Should().Be(v1);

            UnixDate d4 = v1.ToUnixDate();
        }
    }
}
