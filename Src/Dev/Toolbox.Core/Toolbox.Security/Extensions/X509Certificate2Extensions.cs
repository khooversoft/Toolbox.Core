// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Toolbox.Security
{
    public static class X509Certificate2Extensions
    {
        public static bool IsExpired(this X509Certificate2 self)
        {
            self.VerifyNotNull(nameof(self));

            return DateTime.Now > self.NotAfter;
        }
    }
}
