// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;

namespace MicroserviceHost
{
    internal interface IOption
    {
        string AssemblyPath { get; set; }

        TelemetryType ConsoleLevel { get; set; }

        bool Help { get; set; }

        string LoggingFolder { get; set; }

        bool Run { get; set; }

        bool UnRegister { get; set; }

        NamespaceConnection NamespaceConnections { get; }

        public string? Namespace { get; }

        public string? NetworkId { get; }

        IPropertyResolver Properties { get; }

        ITelemetrySecretManager SecretManager { get; }
    }
}