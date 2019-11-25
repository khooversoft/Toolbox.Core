using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public class BlobStoreConnection
    {
        public BlobStoreConnection(string containerName, string connectionString)
        {
            containerName.Verify(nameof(containerName)).IsNotEmpty();
            connectionString.Verify(nameof(connectionString)).IsNotEmpty();

            ContainerName = containerName;
            ConnectionString = connectionString;
        }

        public string ContainerName { get; }

        public string ConnectionString { get; set; }
    }
}
