// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Toolbox.Standard.Test.Extensions
{
    public class ConvertToExtensionTests
    {
        [Fact]
        public void GivenNullClass_WhenConvertToKeyValue_ShouldThrowException()
        {
            TestClass? testClass = null;

            Action subject = () => testClass!.ToKeyValues();

            subject.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenClassWithProperties_WhenConvertToKeyValueForNotNullableClass_ShouldReturnValuesSet()
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
                .ToKeyValues()
                .ToDictionary(x => x.Key, x => x.Value);

            properties.Should().NotBeNull();

            testClass.StringValue.Should().Be(properties[nameof(testClass.StringValue)].ConvertToType<string>());
            testClass.BoolValue.Should().Be(properties[nameof(testClass.BoolValue)].ConvertToType<bool>());
            testClass.IntValue.Should().Be(properties[nameof(testClass.IntValue)].ConvertToType<int>());
            testClass.DoubleValue.Should().Be(properties[nameof(testClass.DoubleValue)].ConvertToType<double>());
            testClass.DateTimeValue.Should().Be(properties[nameof(testClass.DateTimeValue)].ConvertToType<DateTime>());
            testClass.TimeSpanValue.Should().Be(properties[nameof(testClass.TimeSpanValue)].ConvertToType<TimeSpan>());
            testClass.GuidValue.Should().Be(properties[nameof(testClass.GuidValue)].ConvertToType<Guid>());
        }

        [Fact]
        public void GivenClassWithProperties_WhenConvertToKeyValueForNullableClass_ShouldReturnValuesSet()
        {
            var testClass = new TestClass_Nullable
            {
                StringValue = "stringValue_123",
                BoolValue = false,
                IntValue = 101,
                DoubleValue = 205f,
                DateTimeValue = DateTime.Now.AddDays(1),
                TimeSpanValue = TimeSpan.FromSeconds(10),
                GuidValue = Guid.NewGuid(),
            };

            IReadOnlyDictionary<string, object> properties = testClass
                .ToKeyValues()
                .ToDictionary(x => x.Key, x => x.Value);

            properties.Should().NotBeNull();

            testClass.StringValue.Should().Be(properties[nameof(testClass.StringValue)].ConvertToType<string>());
            testClass.BoolValue.Should().Be(properties[nameof(testClass.BoolValue)].ConvertToType<bool>());
            testClass.IntValue.Should().Be(properties[nameof(testClass.IntValue)].ConvertToType<int>());
            testClass.DoubleValue.Should().Be(properties[nameof(testClass.DoubleValue)].ConvertToType<double>());
            testClass.DateTimeValue.Should().Be(properties[nameof(testClass.DateTimeValue)].ConvertToType<DateTime>());
            testClass.TimeSpanValue.Should().Be(properties[nameof(testClass.TimeSpanValue)].ConvertToType<TimeSpan>());
            testClass.GuidValue.Should().Be(properties[nameof(testClass.GuidValue)].ConvertToType<Guid>());
        }

        private class TestClass
        {
            public string? StringValue { get; set; }
            public bool BoolValue { get; set; }
            public int IntValue { get; set; }
            public Double DoubleValue { get; set; }
            public DateTime DateTimeValue { get; set; }
            public TimeSpan TimeSpanValue{ get; set; }
            public Guid GuidValue { get; set; }
        }

        private class TestClass_Nullable
        {
            public string? StringValue { get; set; }
            public bool? BoolValue { get; set; }
            public int? IntValue { get; set; }
            public Double? DoubleValue { get; set; }
            public DateTime? DateTimeValue { get; set; }
            public TimeSpan? TimeSpanValue { get; set; }
            public Guid? GuidValue { get; set; }
        }
    }
}
