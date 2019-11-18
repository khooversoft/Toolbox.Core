// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Khooversoft.Toolbox.Extensions.Configuration;
using Khooversoft.Toolbox.Standard;
using System.IO;

namespace EventHubPerformanceTest
{
    internal class Option : IOption
    {
        [Option("Display help")]
        public bool Help { get; private set; }

        [Option("Send events")]
        public bool Send { get; private set; }

        [Option("Receive events")]
        public bool Receive { get; private set; }

        [Option("Number of events to send")]
        public int Count { get; private set; } = 1;

        [Option("Number tasks to use in sending")]
        public int TaskCount { get; private set; } = 5;

        [Option("Logging folder path where all log files are written to.", "  Default is temp folder + program name.")]
        public string LoggingFolder { get; private set; } = Path.Combine(Path.GetTempPath(), $"{nameof(EventHubPerformanceTest)}_{Guid.NewGuid()}");

        [Option("Set the trace level for console. [Critical, Error, Warning, Informational, or Verbose]")]
        public TelemetryType ConsoleLevel { get; private set; } = TelemetryType.Informational;

        [Option("Event Hub")]
        public EventHub? EventHub { get; private set; }

        [Option("Storage account")]
        public StorageAccount? StorageAccount { get; private set; }

        public static IOption Build(string[] args)
        {
            IOption option = new ConfigurationBuilder()
                .AddIncludeFiles(args, "ConfigFile")
                .AddCommandLine(args.ConflateKeyValue<Option>())
                .Build()
                .BuildOption<Option>();

            if ( option.Help) { return option; }

            option.Verify(nameof(option)).IsNotNull();
            (option.Send || option.Receive).Verify().Assert("Send and/or Receive must be specified");
            option.EventHub.Verify().IsNotNull("Must specify Event hub details");
            option.EventHub!.ConnectionString!.Verify().IsNotEmpty("Event hub connection string is required");
            option.EventHub!.Name!.Verify().IsNotEmpty("Event hub name is required");
            option.EventHub!.ConsumerGroupName!.Verify().IsNotEmpty("Event hub consumer group name is required");
            option.Count.Verify().Assert(x => x >= 0, "Count must be greater then 0, or 0 for no limit");

            (option.Send || option.Receive).Verify().Assert(x => x == true, "Must specify 'Send' or 'Receive'");

            if (option.Receive)
            {
                option.StorageAccount.Verify().IsNotNull("Storage account details are required");
                option.StorageAccount!.AccountName!.Verify().IsNotNull("Storage account name is required");
                option.StorageAccount.ContainerName.Verify().IsNotNull("Storage account container name is required");
                option.StorageAccount.AccountKey.Verify().IsNotNull("Storage account key is required");
            }

            return option;
        }
    }
}
