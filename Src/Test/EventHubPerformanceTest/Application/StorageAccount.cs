using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Core.Extensions.Configuration;

namespace EventHubPerformanceTest
{
    public class StorageAccount
    {
        [Option("Storage account name")]
        public string AccountName { get; set; }

        [Option("Storage account key")]
        public string AccountKey { get; set; }

        [Option("Storage container name")]
        public string ContainerName { get; set; }

        /// <summary>
        /// Calculate connection string
        /// </summary>
        public string ConnectionString => $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey}";
    }
}
