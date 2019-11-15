// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;
using Khooversoft.Toolbox.Standard;
using FluentAssertions;
using System.Linq;

namespace Toolbox.Standard.Test.Extensions
{
    public class ConvertToExtensionsPathValueTests
    {
        [Fact]
        public void GivenNullClass_WhenConvertedToPathValues_ShouldThrowException()
        {
            TestClass? testClass = null;

            Action subject = () => testClass.SerializeToKeyValue();

            subject.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenPopulatedClass_WhenConvertedToPathValues_ReturnTheCorrectSet()
        {
            var testClass = new TestClass
            {
                StringValue = "stringValue",
                BoolValue = true,
                IntValue = 10,
                DoubleValue = 105f,
                DateTimeValue = DateTime.Now,
                TimeSpanValue = TimeSpan.FromSeconds(1),
                GuidValue = Guid.NewGuid(),
            };

            IReadOnlyDictionary<string, object> properties = testClass
                .SerializeToKeyValue()
                .ToDictionary(x => x.Key, x => x.Value);

            properties.Should().NotBeNull();
            properties.Count.Should().Be(7);

            testClass.StringValue.Should().Be(properties[nameof(testClass.StringValue)].ConvertToType<string>());
            testClass.BoolValue.Should().Be(properties[nameof(testClass.BoolValue)].ConvertToType<bool>());
            testClass.IntValue.Should().Be(properties[nameof(testClass.IntValue)].ConvertToType<int>());
            testClass.DoubleValue.Should().Be(properties[nameof(testClass.DoubleValue)].ConvertToType<double>());
            testClass.DateTimeValue.Should().Be(properties[nameof(testClass.DateTimeValue)].ConvertToType<DateTime>());
            testClass.TimeSpanValue.Should().Be(properties[nameof(testClass.TimeSpanValue)].ConvertToType<TimeSpan>());
            testClass.GuidValue.Should().Be(properties[nameof(testClass.GuidValue)].ConvertToType<Guid>());

            properties.ContainsKey(nameof(testClass.Class1)).Should().BeFalse();
            properties.ContainsKey(nameof(testClass.Class2)).Should().BeFalse();
        }

        [Fact]
        public void GivenPopulatedClassWithSubClass_WhenConvertedToPathValues_ReturnTheCorrectSet()
        {
            var testClass = new TestClass
            {
                StringValue = "stringValue",
                BoolValue = true,
                IntValue = 10,
                DoubleValue = 105f,
                DateTimeValue = DateTime.Now,
                TimeSpanValue = TimeSpan.FromSeconds(1),
                GuidValue = Guid.NewGuid(),
                Class1 = new SubTestClass
                {
                    SubName = "class1:SubName",
                    SubNameValue = 10,
                },
                Class2 = new SubTestClass
                {
                    SubName = "class2:SubName",
                    SubNameValue = 20,
                }
            };

            IReadOnlyDictionary<string, object> properties = testClass
                .SerializeToKeyValue()
                .ToDictionary(x => x.Key, x => x.Value);

            properties.Should().NotBeNull();
            properties.Count.Should().Be(11);

            testClass.StringValue.Should().Be(properties[nameof(testClass.StringValue)].ConvertToType<string>());
            testClass.BoolValue.Should().Be(properties[nameof(testClass.BoolValue)].ConvertToType<bool>());
            testClass.IntValue.Should().Be(properties[nameof(testClass.IntValue)].ConvertToType<int>());
            testClass.DoubleValue.Should().Be(properties[nameof(testClass.DoubleValue)].ConvertToType<double>());
            testClass.DateTimeValue.Should().Be(properties[nameof(testClass.DateTimeValue)].ConvertToType<DateTime>());
            testClass.TimeSpanValue.Should().Be(properties[nameof(testClass.TimeSpanValue)].ConvertToType<TimeSpan>());
            testClass.GuidValue.Should().Be(properties[nameof(testClass.GuidValue)].ConvertToType<Guid>());

            testClass.Class1.Should().NotBeNull();
            testClass.Class1.SubName.Should().Be(properties["Class1:SubName"].ConvertToType<string>());
            testClass.Class1.SubNameValue.Should().Be(properties["Class1:SubNameValue"].ConvertToType<int>());

            testClass.Class2.Should().NotBeNull();
            testClass.Class2.SubName.Should().Be(properties["Class2:SubName"].ConvertToType<string>());
            testClass.Class2.SubNameValue.Should().Be(properties["Class2:SubNameValue"].ConvertToType<int>());
        }

        private class TestClass
        {
            public string? StringValue { get; set; }
            public bool BoolValue { get; set; }
            public int IntValue { get; set; }
            public Double DoubleValue { get; set; }
            public DateTime DateTimeValue { get; set; }
            public TimeSpan TimeSpanValue { get; set; }
            public Guid GuidValue { get; set; }

            public SubTestClass? Class1 { get; set; }
            public SubTestClass? Class2 { get; set; }
        }

        private class SubTestClass
        {
            public string? SubName { get; set; }
            public int SubNameValue { get; set; }
        }
    }
}
