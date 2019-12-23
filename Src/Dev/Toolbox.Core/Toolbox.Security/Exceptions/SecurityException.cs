// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Security
{

    [Serializable]
    public class SecurityException : Exception
    {
        public SecurityException() { }
        public SecurityException(string message) : base(message) { }
        public SecurityException(string message, Exception inner) : base(message, inner) { }
        protected SecurityException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
