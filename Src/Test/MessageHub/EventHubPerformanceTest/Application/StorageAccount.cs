// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Khooversoft.Toolbox.Configuration;

namespace EventHubPerformanceTest
{
    internal class StorageAccount
    {
        [Option("Storage account name")]
        public string? AccountName { get; set; }

        [Option("Storage account key")]
        public string? AccountKey { get; set; }

        [Option("Storage container name")]
        public string? ContainerName { get; set; }

        /// <summary>
        /// Calculate connection string
        /// </summary>
        public string ConnectionString => $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey}";
    }
}
