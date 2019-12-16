// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class PropertyResolverTest
    {
        [Fact]
        public void SimplePropertyTest()
        {
            var properties = new Dictionary<string, string>
            {
                ["firstName"] = "Fred",
                ["lastName"] = "Johnson",
                ["flagTest"] = "",
            };

            var tests = new List<(string? Source, string? Resolved)>
            {
                ((string?)null, (string?)null),
                ("", ""),
                ("Simple string", "Simple string"),
                ("{firstName}", "Fred"),
                ("{flagTest}", ""),
                ("Escape {{firstName}} end", "Escape {firstName} end"),
                ("My name is  {firstName}", "My name is  Fred"),
                ("My name is {firstName}, {lastName}", "My name is Fred, Johnson"),
                ("My name is {firstName}, {lastName} end", "My name is Fred, Johnson end"),
            };

            var resolver = new PropertyResolver(properties);

            foreach (var item in tests)
            {
                string resultValue = resolver.Resolve(item.Source!);
                resultValue.Should().Be(item.Resolved);
            }
        }

        [Fact]
        public void ComplexPropertyTest()
        {
            var properties = new Dictionary<string, string>
            {
                ["firstName"] = "Fred",
                ["lastName"] = "Johnson",
                ["fullName"] = "Fullname: {firstName}, {lastName}"
            };

            var tests = new List<(string Source, string Resolved)>
            {
                ("Escape {{firstName}} end", "Escape {firstName} end"),
                ("Escape firstName}} end", "Escape firstName} end"),
            };

            var resolver = new PropertyResolver(properties);

            foreach (var item in tests)
            {
                string resultValue = resolver.Resolve(item.Source);
                resultValue.Should().Be(item.Resolved);
            }
        }

        [Fact]
        public void ComplexPropertyCaseInsensiveTest()
        {
            var properties = new Dictionary<string, string>()
            {
                ["firstName"] = "Fred",
                ["lastName"] = "Johnson",
                ["fullName"] = "Fullname: {firstName}, {lastName}"
            };

            var tests = new List<(string Source, string Resolved)>
            {
                ("My name is  {firstName}", "My name is  Fred"),
                ("My name is {firstName}, {LASTName}", "My name is Fred, Johnson"),
                ("My name is {FirstName}, {lastName} end", "My name is Fred, Johnson end"),
                ("My full name is {FullName} end", "My full name is Fullname: Fred, Johnson end"),
            };

            var resolver = new PropertyResolver(properties, StringComparer.OrdinalIgnoreCase);

            foreach (var item in tests)
            {
                string resultValue = resolver.Resolve(item.Source);
                resultValue.Should().Be(item.Resolved);
            }
        }

        [Fact]
        public void BuildWithAttributeTest()
        {
            var g = Guid.NewGuid();
            var og = Guid.NewGuid();

            var tc = new TestClass
            {
                Name = "Name1",
                Value = 1,
                OptionalValue = 2,
                Key = g,
                OptionalKey = og,
                SwitchValue = true,
                OptionalSwitchValue = false,
                Type = SectionType.Public,
                OptionalType = SectionType.Private,
                ValueList = new string[]
                {
                    "ValueList_1",
                    "ValueList_2",
                    "ValueList_3",
                },

                SubClass1 = new SubClass
                {
                    Name = "Name2",
                    Value = 3,
                    OptionalValue = 4,
                    Key = g,
                    OptionalKey = og,
                    SwitchValue = false,
                    OptionalSwitchValue = true,
                    Type = SectionType.Private,
                    OptionalType = SectionType.Public,
                },

                SubClass2 = new SubClass
                {
                    Name = "Name3",
                    Value = 4,
                    OptionalValue = 5,
                    Key = g,
                    OptionalKey = og,
                    SwitchValue = false,
                    OptionalSwitchValue = true,
                    Type = SectionType.Public,
                    OptionalType = SectionType.Private,
                }
            };

            IPropertyResolver resolver = tc.BuildResolver();

            List<KeyValuePair<string, string>> testValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(nameof(TestClass.Name), "Name1"),
                new KeyValuePair<string, string>(nameof(TestClass.Value), "1"),
                new KeyValuePair<string, string>(nameof(TestClass.OptionalValue), "2"),
                new KeyValuePair<string, string>(nameof(TestClass.Key), g.ToString()),
                new KeyValuePair<string, string>(nameof(TestClass.OptionalKey), og.ToString()),
                new KeyValuePair<string, string>(nameof(TestClass.SwitchValue), "True"),
                new KeyValuePair<string, string>(nameof(TestClass.OptionalSwitchValue), "False"),
                new KeyValuePair<string, string>(nameof(TestClass.Type), "Public"),
                new KeyValuePair<string, string>(nameof(TestClass.OptionalType), "Private"),
                new KeyValuePair<string, string>(nameof(TestClass.ValueList) + ":0", "ValueList_1"),
                new KeyValuePair<string, string>(nameof(TestClass.ValueList) + ":1", "ValueList_2"),
                new KeyValuePair<string, string>(nameof(TestClass.ValueList) + ":2", "ValueList_3"),

                new KeyValuePair<string, string>(nameof(TestClass.SubClass1) + ":" + nameof(TestClass.Name), "Name2"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass1) + ":" + nameof(TestClass.Value), "3"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass1) + ":" + nameof(TestClass.OptionalValue), "4"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass1) + ":" + nameof(TestClass.Key), g.ToString()),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass1) + ":" + nameof(TestClass.OptionalKey), og.ToString()),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass1) + ":" + nameof(TestClass.SwitchValue), "False"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass1) + ":" + nameof(TestClass.OptionalSwitchValue), "True"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass1) + ":" + nameof(TestClass.Type), "Private"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass1) + ":" + nameof(TestClass.OptionalType), "Public"),

                new KeyValuePair<string, string>(nameof(TestClass.SubClass2) + ":" + nameof(TestClass.Name), "Name3"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass2) + ":" + nameof(TestClass.Value), "4"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass2) + ":" + nameof(TestClass.OptionalValue), "5"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass2) + ":" + nameof(TestClass.Key), g.ToString()),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass2) + ":" + nameof(TestClass.OptionalKey), og.ToString()),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass2) + ":" + nameof(TestClass.SwitchValue), "False"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass2) + ":" + nameof(TestClass.OptionalSwitchValue), "True"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass2) + ":" + nameof(TestClass.Type), "Public"),
                new KeyValuePair<string, string>(nameof(TestClass.SubClass2) + ":" + nameof(TestClass.OptionalType), "Private"),
            };

            resolver.SourceProperties.Count.Should().Be(testValues.Count);
            resolver.SourceProperties
                .Zip(testValues, (o, i) => new { o, i })
                .All(x => x.o.Key == x.i.Key && x.o.Value == x.i.Value)
                .Should().BeTrue();
        }

        private enum SectionType
        {
            Private,
            Public
        }

        private class TestClass
        {
            [PropertyResolver]
            public string? Name { get; set; }

            [PropertyResolver]
            public int Value { get; set; }

            [PropertyResolver]
            public int? OptionalValue { get; set; }

            [PropertyResolver]
            public Guid Key { get; set; }

            [PropertyResolver]
            public Guid? OptionalKey { get; set; }

            [PropertyResolver]
            public bool SwitchValue { get; set; }

            [PropertyResolver]
            public bool? OptionalSwitchValue { get; set; }

            [PropertyResolver]
            public SectionType Type { get; set; }

            [PropertyResolver]
            public SectionType? OptionalType { get; set; }

            [PropertyResolver]
            public SubClass? SubClass1 { get; set; }

            [PropertyResolver]
            public SubClass? SubClass2 { get; set; }

            [PropertyResolver]
            public string[]? ValueList { get; set; }
        }

        private class SubClass
        {
            [PropertyResolver]
            public string? Name { get; set; }

            [PropertyResolver]
            public int Value { get; set; }

            [PropertyResolver]
            public int? OptionalValue { get; set; }

            [PropertyResolver]
            public Guid Key { get; set; }

            [PropertyResolver]
            public Guid? OptionalKey { get; set; }

            [PropertyResolver]
            public bool SwitchValue { get; set; }

            [PropertyResolver]
            public bool? OptionalSwitchValue { get; set; }

            [PropertyResolver]
            public SectionType Type { get; set; }

            [PropertyResolver]
            public SectionType? OptionalType { get; set; }
        }
    }
}
