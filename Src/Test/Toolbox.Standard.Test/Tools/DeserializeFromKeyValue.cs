using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Khooversoft.Toolbox.Standard;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Toolbox.Standard.Test.Tools
{
    public class DeserializeFromKeyValue
    {
        [Fact]
        public void GivenSimpleKeyValues_WhenDeserialize_ShouldPass()
        {
            var keyValues = new Dictionary<string, object>
            {
                ["IntValue"] = 2,
                ["StrValue"] = "string3",
                ["ClassType"] = "Second",
            };

            Main subject = keyValues.DeserializeFromKeyValue<Main>();

            subject.IntValue.Should().Be(keyValues[nameof(subject.IntValue)].ConvertToType<int>());
            subject.StrValue.Should().Be(keyValues[nameof(subject.StrValue)].ConvertToType<string>());
            subject.ClassType.Should().Be(keyValues[nameof(subject.ClassType)].ConvertToType<ClassType>());
        }

        [Fact]
        public void GivenSubClassesValues_WhenDeserialize_ShouldPass()
        {
            var keyValues = new Dictionary<string, object>
            {
                ["IntValue"] = 2,
                ["StrValue"] = "string3",
                ["ClassType"] = "Second",
                ["SubClass1:ClassName"] = "ClassName1",
                ["SubClass1:SubValue"] = "10",
                ["SubClass2:ClassName"] = "ClassName2",
                ["SubClass2:SubValue"] = "100",
            };

            Main subject = keyValues.DeserializeFromKeyValue<Main>();

            subject.IntValue.Should().Be(keyValues[nameof(subject.IntValue)].ConvertToType<int>());
            subject.StrValue.Should().Be(keyValues[nameof(subject.StrValue)].ConvertToType<string>());
            subject.ClassType.Should().Be(keyValues[nameof(subject.ClassType)].ConvertToType<ClassType>());

            subject.SubClass1.Should().NotBeNull();
            subject.SubClass1!.ClassName.Should().Be(keyValues["SubClass1:ClassName"].ConvertToType<string>());
            subject.SubClass1!.SubValue.Should().Be(keyValues["SubClass1:SubValue"].ConvertToType<int>());

            subject.SubClass2.Should().NotBeNull();
            subject.SubClass2!.ClassName.Should().Be(keyValues["SubClass2:ClassName"].ConvertToType<string>());
            subject.SubClass2!.SubValue.Should().Be(keyValues["SubClass2:SubValue"].ConvertToType<int>());
        }

        //[Fact]
        //public void GivenStringListValues_WhenDeserialize_ShouldPass()
        //{
        //    var keyValues = new Dictionary<string, object>
        //    {
        //        ["IntValue"] = 2,
        //        ["StrValue"] = "string3",
        //        ["ClassType"] = "Second",
        //        ["SubClass1:ClassName"] = "ClassName1",
        //        ["SubClass1:SubValue"] = "10",
        //        ["SubClass2:ClassName"] = "ClassName2",
        //        ["SubClass2:SubValue"] = "100",
        //        ["Lines:0"] = "Line #1",
        //        ["Lines:1"] = "Line #2"
        //    };

        //    Main subject = keyValues.DeserializeFromKeyValue<Main>();

        //    subject.IntValue.Should().Be(keyValues[nameof(subject.IntValue)].ConvertToType<int>());
        //    subject.StrValue.Should().Be(keyValues[nameof(subject.StrValue)].ConvertToType<string>());
        //    subject.ClassType.Should().Be(keyValues[nameof(subject.ClassType)].ConvertToType<ClassType>());

        //    subject.SubClass1.Should().NotBeNull();
        //    subject.SubClass1!.ClassName.Should().Be(keyValues["SubClass1:ClassName"].ConvertToType<string>());
        //    subject.SubClass1!.SubValue.Should().Be(keyValues["SubClass1:SubValue"].ConvertToType<int>());

        //    subject.SubClass2.Should().NotBeNull();
        //    subject.SubClass2!.ClassName.Should().Be(keyValues["SubClass2:ClassName"].ConvertToType<string>());
        //    subject.SubClass2!.SubValue.Should().Be(keyValues["SubClass2:SubValue"].ConvertToType<int>());

        //    subject.Lines.Should().NotBeNull();
        //    subject.Lines![0].Should().Be(keyValues["Line:0"].ConvertToType<string>());
        //    subject.Lines![1].Should().Be(keyValues["Line:1"].ConvertToType<string>());
        //}
        
        [Fact]
        public void GivenStringListValues_WhenDeserializeWithBind_ShouldPass()
        {
            var keyValues = new Dictionary<string, string>
            {
                ["IntValue"] = "2",
                ["StrValue"] = "string3",
                ["ClassType"] = "Second",
                ["SubClass1:ClassName"] = "ClassName1",
                ["SubClass1:SubValue"] = "10",
                ["SubClass2:ClassName"] = "ClassName2",
                ["SubClass2:SubValue"] = "100",
                ["Lines:0"] = "Line #1",
                ["Lines:1"] = "Line #2"
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(keyValues)
                .Build();

            Main subject = new Main();
            configuration.Bind(subject);

            subject.IntValue.Should().Be(keyValues[nameof(subject.IntValue)].ConvertToType<int>());
            subject.StrValue.Should().Be(keyValues[nameof(subject.StrValue)].ConvertToType<string>());
            subject.ClassType.Should().Be(keyValues[nameof(subject.ClassType)].ConvertToType<ClassType>());

            subject.SubClass1.Should().NotBeNull();
            subject.SubClass1!.ClassName.Should().Be(keyValues["SubClass1:ClassName"].ConvertToType<string>());
            subject.SubClass1!.SubValue.Should().Be(keyValues["SubClass1:SubValue"].ConvertToType<int>());

            subject.SubClass2.Should().NotBeNull();
            subject.SubClass2!.ClassName.Should().Be(keyValues["SubClass2:ClassName"].ConvertToType<string>());
            subject.SubClass2!.SubValue.Should().Be(keyValues["SubClass2:SubValue"].ConvertToType<int>());

            subject.Lines.Should().NotBeNull();
            subject.Lines![0].Should().Be(keyValues["Lines:0"].ConvertToType<string>());
            subject.Lines![1].Should().Be(keyValues["Lines:1"].ConvertToType<string>());
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
