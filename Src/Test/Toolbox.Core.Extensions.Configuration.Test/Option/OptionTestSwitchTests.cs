// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Core.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.Core.Extensions.Configuration.Test.Option
{
    public class OptionTestSwitchTests
    {
        [Fact]
        public void GivenBooleanOption_WhenHelpIsNotSpecified_HelpShouldBeFalse()
        {
            var args = new string[]
            {
                "FileName=name1",
            };

            Option option = Option.Build(args);

            option.Should().NotBeNull();
            option.Help.Should().BeFalse();
        }

        [Fact]
        public void GivenBooleanOption_WhenHelpWithValue_HelpShouldBeTrue()
        {
            var args = new string[]
            {
                "Help=true",
                "FileName=name1",
            };

            Option option = Option.Build(args);

            option.Should().NotBeNull();
            option.Help.Should().BeTrue();
        }

        [Fact]
        public void GivenBooleanOption_WhenHelpSingle_HelpShouldBeTrue()
        {
            var args = new string[]
            {
                "Help",
                "FileName=name1",
            };

            Option option = Option.Build(args);

            option.Should().NotBeNull();
            option.Help.Should().BeTrue();
        }

        [Fact]
        public void GivenBooleanOption_WhenShortCut_HelpShouldBeTrue()
        {
            var args = new string[]
            {
                "?",
                "FileName=name1",
            };

            Option option = Option.Build(args);

            option.Should().NotBeNull();
            option.Help.Should().BeTrue();
        }

        private class Option
        {
            [Option("Display help", ShortCuts = new string[] { "?" })]
            public bool Help { get; set; }

            public static Option Build(params string[] args)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddCommandLine(args.ConflateKeyValue<Option>())
                    .Build();

                Option option = configuration.BuildOption<Option>();

                return option;
            }
        }
    }
}
