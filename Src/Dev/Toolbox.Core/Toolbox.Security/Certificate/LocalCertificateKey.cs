// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Toolbox.Security
{
    /// <summary>
    /// Local certificate key for certificate stored in Windows certificate store
    /// </summary>
    public class LocalCertificateKey
    {
        public LocalCertificateKey(StoreLocation storeLocation, StoreName storeName, string thumbprint, bool requirePrivateKey)
        {
            thumbprint.Verify(nameof(thumbprint)).IsNotNull();

            StoreLocation = storeLocation;
            StoreName = storeName;
            Thumbprint = thumbprint;
            RequirePrivateKey = requirePrivateKey;
        }

        public StoreLocation StoreLocation { get; }

        public StoreName StoreName { get; }

        public string Thumbprint { get; }

        public bool RequirePrivateKey { get; }

        public override string ToString()
        {
            var list = new List<string>
            {
                StoreLocation.ToString(),
                StoreName.ToString(),
                Thumbprint,
            };

            return "/" + string.Join("/", list);
        }
    }
}
