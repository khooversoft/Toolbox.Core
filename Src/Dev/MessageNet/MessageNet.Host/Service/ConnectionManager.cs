// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageNet.Host
{
    public class ConnectionManager : ConcurrentDictionary<string, ConnectionRegistration>, IConnectionManager
    {
        public ConnectionManager()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public ConnectionManager Add(params ConnectionRegistration[] connectionRegistrations)
        {
            connectionRegistrations
                .ForEach(x => this[x.Namespace] = x);

            return this;
        }

        public string GetConnection(string nameSpace) => this[nameSpace].ConnectionString;
    }
}
