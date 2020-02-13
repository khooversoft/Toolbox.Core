// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    /// <summary>
    /// Route lookup request for a node
    /// </summary>
    public class RouteLookupRequest
    {
        /// <summary>
        /// Network ID to lookup
        /// </summary>
        public string? NetworkId { get; set; }

        /// <summary>
        /// Node ID to lookup
        /// </summary>
        public string? NodeId { get; set; }
    }
}
