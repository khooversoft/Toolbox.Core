// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Host
{
    public class NamespaceRegistration
    {
        public NamespaceRegistration(string nameSpace, string connectionString)
        {
            nameSpace.VerifyNotEmpty(nameof(nameSpace));
            connectionString.VerifyNotEmpty(nameof(connectionString));

            Namespace = nameSpace;
            ConnectionString = connectionString;
        }

        public string Namespace { get; }

        public string ConnectionString { get; }
    }
}
