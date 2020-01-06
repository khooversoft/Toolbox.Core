// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MicroserviceHost
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
        [PropertyResolver]
        public string NodeId { get; set; } = string.Empty;

        [Option("Service Bus connection string")]
        [PropertyResolver]
        public string ServiceBusConnection { get; set; } = string.Empty;

        [Option("Name Server URI for registrations")]
        [PropertyResolver]
        public string NameServerUri { get; set; } = string.Empty;

        [Option("Path to assembly to run")]
        [PropertyResolver]
        public string AssemblyPath { get; set; } = string.Empty;

        [Option("Shared access key for service bus connection string")]
        [PropertyResolver]
        [TelemetrySecret]
        public string SharedAccessKey { get; set; } = string.Empty;

        [Option("Logging folder path where all log files are written to.", "  Default is temp folder + program name.")]
        public string LoggingFolder { get; set; } = Path.Combine(Path.GetTempPath(), $"{nameof(MicroserviceHost)}_{Guid.NewGuid()}");

        [Option("Set the trace level for console. [Critical, Error, Warning, Informational, or Verbose]")]
        public TelemetryType ConsoleLevel { get; set; } = TelemetryType.Informational;

        public IPropertyResolver Properties { get; set; } = null!;

        public ITelemetrySecretManager SecretManager { get; set; } = null!;

        public static IOption Build(string[] args)
        {
            Option option = new ConfigurationBuilder()
                .AddIncludeFiles(args, "ConfigFile")
                .AddCommandLine(args.ConflateKeyValue<Option>())
                .AddUserSecrets(nameof(MicroserviceHost))
                .Build()
                .BuildOption<Option>();

            if (option.Help) { return option; }

            option.Verify(nameof(option)).IsNotNull();
            option.Run.Verify().Assert("Run or UnRegister must be specified");
            option.NodeId.Verify().IsNotEmpty($"{nameof(option.NodeId)} is required");
            option.ServiceBusConnection.Verify().IsNotEmpty($"{option.ServiceBusConnection} is required");
            option.NameServerUri.Verify().IsNotEmpty($"{option.NameServerUri} is required");

            option.AssemblyPath!
                .Verify()
                .IsNotEmpty($"{option.AssemblyPath} is required")
                .Assert(x => File.Exists(x), $"{option.AssemblyPath} does not exist");

            option.Properties = option.BuildResolver();
            option.SecretManager = option.BuildSecretManager();

            return option;
        }
    }
}
