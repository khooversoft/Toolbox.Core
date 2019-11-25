// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Configuration
{
    public class KeyVaultConfiguration
    {
        public KeyVaultConfiguration(string keyVaultName, string aadClientId, string aadClientSecret)
        {
            KeyVaultName = keyVaultName;
            AadClientId = aadClientId;
            AadClientSecret = aadClientSecret;
        }

        [Option("Azure KeyVault name (required).")]
        public string KeyVaultName { get; set; }

        [Option("Azure Active Directory Client ID (Application ID) (required).")]
        public string AadClientId { get; set; }

        [Option("Azure Active Directory Client secret. (required).")]
        //[TelemetrySecret]
        public string AadClientSecret { get; set; }

        [Option("KeyVault keys to property map (required).")]
        public IReadOnlyDictionary<string, string>? Keys { get; set; }
    }
}
