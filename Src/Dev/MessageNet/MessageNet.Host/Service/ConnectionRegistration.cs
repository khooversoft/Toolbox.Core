// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Host
{
    public class ConnectionRegistration
    {
        public ConnectionRegistration(string nameSpace, string connectionString)
        {
            nameSpace.Verify(nameof(nameSpace)).IsNotEmpty();
            connectionString.Verify(nameof(connectionString)).IsNotEmpty();

            Namespace = nameSpace;
            ConnectionString = connectionString;
        }

        public string Namespace { get; }

        public string ConnectionString { get; }
    }
}
