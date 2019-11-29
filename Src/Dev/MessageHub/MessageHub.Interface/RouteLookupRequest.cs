using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageHub.Interface
{
    /// <summary>
    /// Route lookup request for a node
    /// </summary>
    public class RouteLookupRequest
    {
        /// <summary>
        /// Node ID to lookup
        /// </summary>
        public string? SearchNodeId { get; set; }
    }
}
