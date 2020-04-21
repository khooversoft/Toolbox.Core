// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Azure
{
    public class BlobStoreConnection
    {
        public BlobStoreConnection(string containerName, string connectionString)
        {
            containerName.VerifyNotEmpty(nameof(containerName));
            connectionString.VerifyNotEmpty(nameof(connectionString));

            ContainerName = containerName;
            ConnectionString = connectionString;
        }

        public string ContainerName { get; }

        public string ConnectionString { get; }
    }
}
