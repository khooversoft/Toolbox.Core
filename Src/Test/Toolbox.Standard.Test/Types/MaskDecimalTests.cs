using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Types
{
    public class MaskDecimalTests
    {
        [Fact]
        public void GivenDouble_WhenToMask4_ShouldRoundTrip()
        {
            const double v1 = 100383.3829;

            MaskDecimal4 m1 = (MaskDecimal4)v1;
            MaskDecimal4 m2 = new MaskDecimal4(v1);
            m1.Value.Should().Be(m2.Value);

            double v2 = m1;
            v2.Should().Be(v1);

            double v3 = m2.ToDouble();
            v3.Should().Be(v1);
        }
    }
}
