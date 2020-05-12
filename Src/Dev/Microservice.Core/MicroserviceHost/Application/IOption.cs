// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;

namespace MicroserviceHost
{
    public interface IOption
    {
        string AssemblyPath { get; set; }

        TelemetryType ConsoleLevel { get; set; }

        bool Help { get; set; }

        string LoggingFolder { get; set; }

        bool Run { get; set; }

        bool UnRegister { get; set; }

        IList<NamespaceConnection> NamespaceConnections { get; }

        public string Namespace { get; }

        public string NetworkId { get; }

        IPropertyResolver Properties { get; }

        ITelemetrySecretManager SecretManager { get; }

        IMessageNetConfig MessageNetConfig { get; }
    }
}