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
    public class RouteRegistrationResponse
    {
        /// <summary>
        /// Network ID
        /// </summary>
        public string? NetworkId { get; set; }

        /// <summary>
        /// Node id
        /// </summary>
        public string? NodeId { get; set; }

        /// <summary>
        /// Node's input queue URI, queue to receive messages
        /// </summary>
        public string? InputQueueUri { get; set; }
    }
}
