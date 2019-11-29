// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageHub.Management
{
    public class ServiceBusConnection
    {
        public ServiceBusConnection(string connectionString)
        {
            connectionString.Verify(nameof(connectionString)).IsNotEmpty();

            ConnectionString = connectionString;

            // TODO: need to extract service bus name
            ServiceBusName = new ServiceBusConnectionStringBuilder(ConnectionString).Endpoint;
        }

        public string ConnectionString { get; }

        public string ServiceBusName { get; }
    }
}
