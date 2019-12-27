// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Management
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

        public string ConnectionString { get; }
    }
}
