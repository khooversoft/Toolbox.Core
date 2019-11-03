using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class TestDimensionsTests
    {
        [Fact]
        public void TestDimensionsTest()
        {
            IWorkContext wrk = new WorkContextBuilder()
            {
                Dimensions = new EventDimensionsBuilder()
                    .Add("Key1", "Value1")
                    .Add("Key2", "Value2")
                    .Build(),
            }.Build();

            wrk.Dimensions.Count.Should().Be(2);
            wrk.Dimensions["Key1"].Should().Be("Value1");
            wrk.Dimensions["Key2"].Should().Be("Value2");

            var dim = new EventDimensionsBuilder()
                .Add("Key3", "Value3")
                .Add("Key4", "Value4")
                .Build();

            wrk.Dimensions.Count.Should().Be(2);
            dim["Key3"].Should().Be("Value3");
            dim["Key4"].Should().Be("Value4");

            wrk = wrk.With(dim);
            wrk.Dimensions.Count.Should().Be(4);
            wrk.Dimensions["Key1"].Should().Be("Value1");
            wrk.Dimensions["Key2"].Should().Be("Value2");
            wrk.Dimensions["Key3"].Should().Be("Value3");
            wrk.Dimensions["Key4"].Should().Be("Value4");

            var dim2 = new EventDimensionsBuilder()
                .Add("Key3", "Value33")
                .Add("Key4", "Value44")
                .Build();

            wrk = wrk.With(dim2);
            wrk.Dimensions.Count.Should().Be(4);
            wrk.Dimensions["Key1"].Should().Be("Value1");
            wrk.Dimensions["Key2"].Should().Be("Value2");
            wrk.Dimensions["Key3"].Should().Be("Value33");
            wrk.Dimensions["Key4"].Should().Be("Value44");
        }

        [Fact]
        public void DimensionTest()
        {
            var dimensions = new EventDimensionsBuilder()
               .Add("WorkflowCv", new CorrelationVector().ToString())
               .Add("PackageName", "PackageNameData")
               .Add("FriendlyName", "friendlyNameData")
               .Add("BuildNumber", "buildNumberData")
               .Build();

            dimensions.Count.Should().Be(4);

            IEventDimensions addDimensions = new EventDimensionsBuilder()
                .Add("ExeFile", "Exec.exe")
                .Add("NodeType", "LocalExec")
                .Build();

            addDimensions.Count.Should().Be(2);

            IEventDimensions newDimensions = (EventDimensions)dimensions + addDimensions;
            newDimensions.Count.Should().Be(6);
        }
    }
}
