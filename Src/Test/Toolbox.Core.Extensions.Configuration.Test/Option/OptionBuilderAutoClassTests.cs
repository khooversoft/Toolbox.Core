// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Core.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.Core.Extensions.Configuration.Test.Option
{
    public class OptionBuilderAutoClassTests
    {
        [Fact]
        public void FullOptionTest()
        {
            var args = new string[]
            {
                "Help=true",
                "FileName=name1",
                "IntValue=1",
                "OptionalIntValue=2",
                "BoolValue=true",
                "OptionalBoolValue=false",
                "Data1:Name=d1_name",
                "Data1:Value=1",
                "Data1:Type=Private",
                "Data2:Name=d2_name",
                "Data2:Value=2",
                "Data2:Type=Public",
                "Data2:Key=933B757C-CF76-4B0D-8912-BAAD5C0B02A8",
                "StartNames:0=Start1",
                "StartNames:1=Start2",
                "StartNames:2=Start3",
                "NotificationType=Team",
                "Properties:Value1=instance1",
                "Properties:Value2=instance2",
                "Properties:Value3=instance3",
            };

            IOption option = Option.Build(args);

            option.Should().NotBeNull();
            option.Help.Should().BeTrue();
            option.FileName.Should().Be("name1");
            option.StartNames.Should().NotBeNull();
            option.StartNames?.Count.Should().Be(3);
            option.StartNames
                .Zip(new string[] { "Start1", "Start2", "Start3" }, (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();

            option.Data1.Should().NotBeNull();
            option.Data1?.Name.Should().Be("d1_name");
            option.Data1?.Value.Should().Be(1);
            option.Data1?.Type.Should().Be(SectionType.Private);
            option.Data1?.Key.Should().BeNull();

            option.Data2.Should().NotBeNull();
            option.Data2?.Name.Should().Be("d2_name");
            option.Data2?.Value.Should().Be(2);
            option.Data2?.Type.Should().Be(SectionType.Public);
            option.Data2?.Key.Should().Be(Guid.Parse("933B757C-CF76-4B0D-8912-BAAD5C0B02A8"));

            option.NotificationType.Should().Be(NotificationType.Team);

            var expectedProperties = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Value1", "instance1"),
                new KeyValuePair<string, string>("Value2", "instance2"),
                new KeyValuePair<string, string>("Value3", "instance3"),
            };

            option.Properties.OrderBy(x => x.Key)
                .Zip(expectedProperties.OrderBy(x => x.Key), (o, i) => (o, i))
                .All(x => x.o.Key == x.i.Key && x.o.Value == x.i.Value)
                .Should().BeTrue();
        }

        private enum SectionType
        {
            Private,
            Public
        }

        private class SectionData
        {
            [Option("Set name for section")]
            public string? Name { get; set; }

            [Option("Set value for section")]
            public int Value { get; set; }

            [Option("Set key for section")]
            public Guid? Key { get; set; }

            [Option("Set section type")]
            public SectionType? Type { get; set; }
        }

        private enum NotificationType
        {
            Team,
            Requester,
            Disabled,
        }

        private interface IOption
        {
            bool Help { get; }

            string? FileName { get; }

            string? ClientSecret { get; }

            int IntValue { get; }

            int? OptionalIntValue { get; }

            bool BoolValue { get; }

            bool? OptionalBoolValue { get; }

            SectionData? Data1 { get; }

            SectionData? Data2 { get; }

            IReadOnlyList<string>? StartNames { get; }

            NotificationType? NotificationType { get; }

            IDictionary<string, string>? Properties { get; }
        }

        [OptionHelp(HelpArea.Header, "Class help text line #1")]
        [OptionHelp(HelpArea.Header, "Class help text line #2")]
        private class Option : IOption
        {
            [Option("Display help", ShortCuts = new string[] { "?" })]
            public bool Help { get; set; }

            [Option("Set file name", "Second line of text")]
            public string? FileName { get; set; }

            [Option("Set client secret")]
            public string? ClientSecret { get; set; }

            [Option("Set int value")]
            public int IntValue { get; set; }

            [Option("Set int? value")]
            public int? OptionalIntValue { get; set; }

            [Option("Set bool value")]
            public bool BoolValue { get; set; }

            [Option("Set bool? value")]
            public bool? OptionalBoolValue { get; set; }

            [Option("Set 'SectionData' parameters")]
            public SectionData? Data1 { get; set; }

            [Option("Set 'SectionData' parameters")]
            public SectionData? Data2 { get; set; }

            [Option("Start names")]
            public IReadOnlyList<string>? StartNames { get; set; }

            [Option("Notification type")]
            public NotificationType? NotificationType { get; set; }

            [Option("Properties type")]
            public IDictionary<string, string>? Properties { get; set; }

            public static IOption Build(params string[] args)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .Build();

                IOption option = configuration.BuildOption<Option>();

                return option;
            }
        }
    }
}
