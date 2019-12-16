// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Xunit;

namespace Toolbox.Core.Extensions.Configuration.Test.Option
{
    public class IncludeOptionTests
    {
        private readonly string TestJsonFilePath;

        public IncludeOptionTests()
        {
            Stream stream = Assembly.GetAssembly(typeof(IncludeOptionTests))
                .Verify().IsNotNull("Assembly cannot be loaded").Value!
                .GetManifestResourceStream("Toolbox.Configuration.Test.Option.Test.json")!
                .Verify().IsNotNull("Cannot find Test.json in resources")
                .Value;

            TestJsonFilePath = Path.GetTempFileName();
            using (var wr = new StreamWriter(TestJsonFilePath))
            {
                stream.CopyTo(wr.BaseStream);
            }
        }

        [Fact]
        public void GivenOption_WhenConfigFileIsSpecified_ReturnCorrectProperties()
        {
            Option option = Option.Build($"ConfigFile={TestJsonFilePath}");
        }

        private class Option
        {
            [Option("Display help")]
            public bool Help { get; set; }

            [Option("Send events")]
            public bool Send { get; set; }

            [Option("Receive events")]
            public bool Receive { get; set; }

            [Option("Number of events to send")]
            public int Count { get; set; } = 1;

            [Option("Event Hub")]
            public EventHub? EventHub { get; set; }

            [Option("Storage account")]
            public StorageAccount? StorageAccount { get; set; }

            public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

            public static Option Build(params string[] args)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddIncludeFiles(args, "ConfigFile")
                    .AddCommandLine(args.ConflateKeyValue<Option>())
                    .Build();

                Option option = new Option();
                configuration.Bind(option);

                option.Verify(nameof(option)).IsNotNull();
                (option.Send || option.Receive).Verify().Assert("Send and/or Receive must be specified");
                option.EventHub.Verify().IsNotNull("Must specify Event hub details");
                option.EventHub?.ConnectionString.Verify().IsNotEmpty("Event hub connection string is required");
                option.EventHub?.Name.Verify().IsNotEmpty("Event hub name is required");

                if (option.Receive)
                {
                    option.StorageAccount.Verify().IsNotNull("Storage account details are required");
                    option.StorageAccount?.AccountName.Verify().IsNotEmpty("Storage account name is required");
                    option.StorageAccount?.ContainerName.Verify().IsNotEmpty("Storage account container name is required");
                    option.StorageAccount?.AccountKey.Verify().IsNotEmpty("Storage account key is required");
                }

                return option;
            }
        }

        public class EventHub
        {
            [Option("Event hub connection string")]
            public string ConnectionString { get; set; } = string.Empty;

            [Option("Event hub name")]
            public string Name { get; set; } = string.Empty;
        }

        public class StorageAccount
        {
            [Option("Storage account name")]
            public string AccountName { get; set; } = string.Empty;

            [Option("Storage account key")]
            public string AccountKey { get; set; } = string.Empty;

            [Option("Storage container name")]
            public string ContainerName { get; set; } = string.Empty;

            /// <summary>
            /// Calculate connection string
            /// </summary>
            public string ConnectionString => $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey}";
        }
    }
}
