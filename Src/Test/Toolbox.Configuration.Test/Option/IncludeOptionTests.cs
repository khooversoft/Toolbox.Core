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
                .VerifyNotNull("Assembly cannot be loaded")!
                .GetManifestResourceStream("Toolbox.Configuration.Test.Option.Test.json")!
                .VerifyNotNull("Cannot find Test.json in resources");

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

                option.VerifyNotNull(nameof(option));
                (option.Send || option.Receive).VerifyAssert(x => x, "Send and/or Receive must be specified");
                option.EventHub.VerifyNotNull("Must specify Event hub details");
                option.EventHub?.ConnectionString.VerifyNotEmpty("Event hub connection string is required");
                option.EventHub?.Name.VerifyNotEmpty("Event hub name is required");

                if (option.Receive)
                {
                    option.StorageAccount.VerifyNotNull("Storage account details are required");
                    option.StorageAccount?.AccountName.VerifyNotEmpty("Storage account name is required");
                    option.StorageAccount?.ContainerName.VerifyNotEmpty("Storage account container name is required");
                    option.StorageAccount?.AccountKey.VerifyNotEmpty("Storage account key is required");
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
