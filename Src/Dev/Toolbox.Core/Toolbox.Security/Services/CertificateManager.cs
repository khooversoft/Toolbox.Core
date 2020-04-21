// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Khooversoft.Toolbox.Security
{
    /// <summary>
    /// Manage a collection of local certificates
    /// </summary>
    public class CertificateManager : ICertificateManager
    {
        private readonly ConcurrentDictionary<string, LocalCertificate> _registration = new ConcurrentDictionary<string, LocalCertificate>(StringComparer.OrdinalIgnoreCase);

        public CertificateManager()
        {
        }

        public void Clear(IWorkContext context)
        {
            _registration.Clear();
        }

        public void Set(IWorkContext context, LocalCertificate certificate)
        {
            certificate.VerifyNotNull(nameof(certificate));

            _registration[certificate.LocalCertificateKey.Thumbprint] = certificate;
        }

        public LocalCertificate? Get(IWorkContext context, string thumbprint)
        {
            if (!_registration.TryGetValue(thumbprint, out LocalCertificate spec))
            {
                return null;
            }

            return spec;
        }

        public IEnumerator<LocalCertificate> LocalCertificateItems()
        {
            return _registration.Values.GetEnumerator();
        }
    }
}
