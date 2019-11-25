using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServiceBusPerformanceTest
{
    internal class Option : IOption
    {
        [Option("Display help")]
        public bool Help { get; set; }

        [Option("Send events")]
        public bool Send { get; private set; }

        [Option("Receive events")]
        public bool Receive { get; private set; }

        [Option("Number of events to send")]
        public int Count { get; private set; } = 1;

        [Option("Number tasks to use in sending")]
        public int TaskCount { get; private set; } = 5;

        [Option("Logging folder path where all log files are written to.", "  Default is temp folder + program name.")]
        public string LoggingFolder { get; private set; } = Path.Combine(Path.GetTempPath(), $"{nameof(ServiceBusPerformanceTest)}_{Guid.NewGuid()}");

        [Option("Set the trace level for console. [Critical, Error, Warning, Informational, or Verbose]")]
        public TelemetryType ConsoleLevel { get; private set; } = TelemetryType.Informational;

        [Option("Service Bus Connection String")]
        public string? ServiceBusConnectionString { get; set; }

        [Option("Service Bus Queue Name")]
        public string? QueueName { get; set; }

        public static IOption Build(string[] args)
        {
            IOption option = new ConfigurationBuilder()
                .AddIncludeFiles(args, "ConfigFile")
                .AddCommandLine(args.ConflateKeyValue<Option>())
                .Build()
                .BuildOption<Option>();

            if (option.Help) { return option; }

            option.Verify(nameof(option)).IsNotNull();
            (option.Send || option.Receive).Verify().Assert("Send and/or Receive must be specified");
            option.ServiceBusConnectionString.Verify().IsNotNull("Service bus connection string is required");
            option.QueueName!.Verify().IsNotEmpty("Queue name is required");
            option.Count.Verify().Assert(x => x >= 0, "Count must be greater then 0, or 0 for no limit");

            return option;
        }
    }
}
