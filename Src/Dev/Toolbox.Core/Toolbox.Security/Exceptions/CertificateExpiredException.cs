// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Security
{
    [Serializable]
    public class CertificateExpiredException : Exception
    {
        public CertificateExpiredException() { }

        public CertificateExpiredException(string message) : base(message) { }

        public CertificateExpiredException(string message, Exception inner) : base(message, inner) { }

        protected CertificateExpiredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}