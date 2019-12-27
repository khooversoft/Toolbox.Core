// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartBlockServer
{
    internal class Option : IOption
    {
        [Option("Display help")]
        public bool Help { get; set; }

        [Option("Run server")]
        public bool Run { get; set; }

        [Option("Unregister from the Message Net")]
        public bool UnRegister { get; set; }

        [Option("Node ID for Message Net")]
        public string NodeId { get; set; }

        [Option("Service Bus connection string")]
        public string ServiceBusConnection { get; set; }

        [Option("Name Server URI for registrations")]
        public string NameServerUri { get; set; }

        [Option("Logging folder path where all log files are written to.", "  Default is temp folder + program name.")]
        public string LoggingFolder { get; set; } = Path.Combine(Path.GetTempPath(), $"{nameof(SmartBlockServer)}_{Guid.NewGuid()}");

        [Option("Set the trace level for console. [Critical, Error, Warning, Informational, or Verbose]")]
        public TelemetryType ConsoleLevel { get; set; } = TelemetryType.Informational;

        public static IOption Build(string[] args)
        {
            IOption option = new ConfigurationBuilder()
                .AddIncludeFiles(args, "ConfigFile")
                .AddCommandLine(args.ConflateKeyValue<Option>())
                .AddUserSecrets(nameof(SmartBlockServer))
                .Build()
                .BuildOption<Option>();

            if (option.Help) { return option; }

            option.Verify(nameof(option)).IsNotNull();
            option.Run.Verify().Assert("Send and/or Receive must be specified");

            return option;
        }
    }
}
