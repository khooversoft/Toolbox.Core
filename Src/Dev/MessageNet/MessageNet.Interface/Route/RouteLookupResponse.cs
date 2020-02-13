// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class RouteLookupResponse
    {
        public string? NetworkId { get; set; }

        public string? NodeId { get; set; }

        public string? InputUri { get; set; }
    }
}
