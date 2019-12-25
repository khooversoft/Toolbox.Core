// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageHub.Interface
{
    /// <summary>
    /// Node registration response to a request
    /// </summary>
    public class RouteRegistrationResponse
    {
        /// <summary>
        /// Node's input queue URI, queue to receive messages
        /// </summary>
        public string? InputQueueUri { get; set; }

        /// <summary>
        /// Routes to forward all messages
        /// </summary>
        public IReadOnlyList<string>? ForwardRoutes { get; set; }
    }
}
