// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Host
{
    public class ConnectionRegistration
    {
        public ConnectionRegistration(string networkId, string connectionString)
        {
            NetworkId = networkId;
            ConnectionString = connectionString;
        }

        public string NetworkId { get; }

        public string ConnectionString { get; }
    }
}
