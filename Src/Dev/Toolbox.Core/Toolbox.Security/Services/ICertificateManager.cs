// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;

namespace Khooversoft.Toolbox.Security
{
    public interface ICertificateManager
    {
        void Clear();

        void Set(LocalCertificate certificate);

        LocalCertificate? Get(string thumbprint);

        IEnumerator<LocalCertificate> LocalCertificateItems();
    }
}
