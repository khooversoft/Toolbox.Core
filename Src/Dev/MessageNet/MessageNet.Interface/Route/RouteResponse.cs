// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    /// <summary>
    /// Node registration response to a request
    /// </summary>
    public class RouteResponse
    {
        public string? Namespace { get; set; }

        public string? NetworkId { get; set; }

        public string? NodeId { get; set; }
    }
}
