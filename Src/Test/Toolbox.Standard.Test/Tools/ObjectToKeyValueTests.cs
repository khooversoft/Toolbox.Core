using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;
using Khooversoft.Toolbox.Standard;
using FluentAssertions;
using System.Linq;

namespace Toolbox.Standard.Test.Tools
{
    public class ObjectToKeyValueTests
    {
        [Fact]
        public void GivenClassWithSubClassData_WhenDeserialize_Passes()
        {
            var data = new Main
            {
                IntValue = 1,
                StrValue = "value2",
                SubClass1 = new SubClass
                {
                    ClassName = "SubClass1Name",
                    SubValue = 3
                },
                SubClass2 = new SubClass
                {
                    ClassName = "SubClass2Name",
                    SubValue = 4
                },
                SubClasses = new List<SubClass>
                {
                    new SubClass { ClassName = "SubClass3Name", SubValue = 5 },
                    new SubClass
                    {
                        ClassName = "SubClass4Name",
                        SubValue = 6,
                        SubSubClasses = new List<SubSubClass>
                        {
                            new SubSubClass { SubSubName = "SubSubName1" },
                        }
                    },
                }
            };

            IReadOnlyDictionary<string, object>? subject = data.SerializeToKeyValue().ToDictionary(x => x.Key, x => x.Value);
            subject.Should().NotBeNull();
            subject.Count.Should().Be(11);

            subject["IntValue"].Should().Be(data.IntValue);
            subject["StrValue"].Should().Be(data.StrValue);
            subject["SubClass1:ClassName"].Should().Be(data.SubClass1.ClassName);
            subject["SubClass1:SubValue"].Should().Be(data.SubClass1.SubValue);
            subject["SubClass2:ClassName"].Should().Be(data.SubClass2.ClassName);
            subject["SubClass2:SubValue"].Should().Be(data.SubClass2.SubValue);
            subject["SubClasses:0:ClassName"].Should().Be(data.SubClasses.First().ClassName);
            subject["SubClasses:0:SubValue"].Should().Be(data.SubClasses.First().SubValue);
            subject["SubClasses:1:ClassName"].Should().Be(data.SubClasses.Skip(1).First().ClassName);
            subject["SubClasses:1:SubValue"].Should().Be(data.SubClasses.Skip(1).First().SubValue);
            subject["SubClasses:1:SubValue"].Should().Be(data.SubClasses.Skip(1).First().SubValue);
            subject["SubClasses:1:SubSubClasses:0:SubSubName"].Should().Be(data.SubClasses.Skip(1).First().SubSubClasses.First().SubSubName);
        }

        private class Main
        {
            public int IntValue { get; set; }

            public string? StrValue { get; set; }

            public SubClass? SubClass1 { get; set; }

            public SubClass? SubClass2 { get; set; }

            public IReadOnlyList<SubClass>? SubClasses { get; set; }
        }

        public class SubClass
        {
            public string? ClassName { get; set; }

            public int SubValue { get; set; }

            public IReadOnlyList<SubSubClass>? SubSubClasses { get; set; }
        }

        public class SubSubClass
        {
            public string? SubSubName { get; set; }
        }
    }
}
