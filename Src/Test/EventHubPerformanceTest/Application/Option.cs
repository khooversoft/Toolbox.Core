using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Toolbox.Core.Extensions.Configuration;
using Toolbox.Standard;

namespace EventHubPerformanceTest
{
    public class Option : IOption
    {
        [Option("Display help", ShortCuts = new string[] { "?" })]
        public bool Help { get; }

        [Option("Send events")]
        public bool Send { get; }

        [Option("Receive events")]
        public bool Receive { get; }

        [Option("Number of events to send")]
        public int Count { get; } = 1;

        [Option("Event Hub")]
        public EventHub EventHub { get; }

        [Option("Storage account")]
        public StorageAccount StorageAccount { get; }

        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        public static IOption Build(string[] args)
        {
            IOption option = new ConfigurationBuilder()
                .AddIncludeFiles(args)
                .AddCommandLine(args.ConflateKeyValue<Option>())
                .Build()
                .BuildOption<Option>();

            Verify.Assert(option != null, $"Option did not build");
            Verify.Assert(option.Send || option.Receive, "Send and/or Receive must be specified");
            Verify.Assert(option.EventHub != null, "Event hub details are required");
            Verify.Assert(option.EventHub.ConnectionString != null, "Event hub connection string is required");
            Verify.Assert(option.EventHub.Name != null, "Event hub name is required");

            if( option.Receive)
            {
                Verify.Assert(option.StorageAccount != null, "Storage account details are required");
                Verify.Assert(option.StorageAccount.AccountName != null, "Storage account name is required");
                Verify.Assert(option.StorageAccount.ContainerName != null, "Storage account container name is required");
                Verify.Assert(option.StorageAccount.AccountKey != null, "Storage account key is required");
            }

            return option;
        }
    }
}
