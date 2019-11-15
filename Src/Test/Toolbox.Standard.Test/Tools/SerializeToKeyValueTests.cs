// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    public class SerializeToKeyValueTests
    {
        [Fact]
        public void GivenClassData_WhenDeserialize_Passes()
        {
            var data = new Main
            {
                IntValue = 1,
                StrValue = "value2",
                ClassType = ClassType.Thrid,
            };

            IReadOnlyDictionary<string, object> subject = data.SerializeToKeyValue().ToDictionary(x => x.Key, x => x.Value);
            subject.Should().NotBeNull();
            subject.Count.Should().Be(3);

            subject["IntValue"].Should().Be(data.IntValue);
            subject["StrValue"].Should().Be(data.StrValue);
            subject["ClassType"].Should().Be(data.ClassType);
        }

        [Fact]
        public void GivenClassWithSubClassData_WhenDeserialize_Passes()
        {
            var data = new Main
            {
                IntValue = 1,
                StrValue = "value2",
                ClassType = ClassType.Second,
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

            IReadOnlyDictionary<string, object> subject = data.SerializeToKeyValue().ToDictionary(x => x.Key, x => x.Value);
            subject.Should().NotBeNull();
            subject.Count.Should().Be(12);

            subject["IntValue"].Should().Be(data.IntValue);
            subject["StrValue"].Should().Be(data.StrValue);
            subject["ClassType"].Should().Be(data.ClassType);
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

        [Fact]
        public void GivenClassWithListData_WhenDeserialize_Passes()
        {
            var data = new Main
            {
                IntValue = 1,
                StrValue = "value2",
                ClassType = ClassType.First,
                Lines = new string[]
                {
                    "Line #1",
                    "Line #2",
                },
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
                    new SubClass
                    {
                        ClassName = "SubClass3Name",
                        SubValue = 5,
                        IntValues = Enumerable.Range(0, 3).ToList(),
                    },
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

            IReadOnlyDictionary<string, object> subject = data.SerializeToKeyValue().ToDictionary(x => x.Key, x => x.Value);
            subject.Should().NotBeNull();
            subject.Count.Should().Be(17);

            subject["IntValue"].Should().Be(data.IntValue);
            subject["StrValue"].Should().Be(data.StrValue);
            subject["ClassType"].Should().Be(data.ClassType);

            subject["Lines:0"].Should().Be(data.Lines.First());
            subject["Lines:1"].Should().Be(data.Lines.Skip(1).First());

            subject["SubClass1:ClassName"].Should().Be(data.SubClass1.ClassName);
            subject["SubClass1:SubValue"].Should().Be(data.SubClass1.SubValue);
            subject["SubClass2:ClassName"].Should().Be(data.SubClass2.ClassName);
            subject["SubClass2:SubValue"].Should().Be(data.SubClass2.SubValue);

            subject["SubClasses:0:ClassName"].Should().Be(data.SubClasses[0].ClassName);
            subject["SubClasses:0:SubValue"].Should().Be(data.SubClasses[0].SubValue);
            subject["SubClasses:0:IntValues:0"].Should().Be(data.SubClasses[0].IntValues![0]);
            subject["SubClasses:0:IntValues:1"].Should().Be(data.SubClasses[0].IntValues![1]);
            subject["SubClasses:0:IntValues:2"].Should().Be(data.SubClasses[0].IntValues![2]);

            subject["SubClasses:1:ClassName"].Should().Be(data.SubClasses[1].ClassName);
            subject["SubClasses:1:SubValue"].Should().Be(data.SubClasses[1].SubValue);
            subject["SubClasses:1:SubSubClasses:0:SubSubName"].Should().Be(data.SubClasses[1].SubSubClasses.Single().SubSubName);
        }

        private enum ClassType
        {
            First,
            Second,
            Thrid,
        };

        private class Main
        {
            public int IntValue { get; set; }

            public string? StrValue { get; set; }

            public ClassType ClassType { get; set; }

            public SubClass? SubClass1 { get; set; }

            public SubClass? SubClass2 { get; set; }

            public IReadOnlyList<string>? Lines { get; set; }

            public IReadOnlyList<SubClass>? SubClasses { get; set; }
        }

        public class SubClass
        {
            public string? ClassName { get; set; }

            public int SubValue { get; set; }

            public IReadOnlyList<SubSubClass>? SubSubClasses { get; set; }

            public IList<int>? IntValues { get; set; }
        }

        public class SubSubClass
        {
            public string? SubSubName { get; set; }
        }
    }
}
