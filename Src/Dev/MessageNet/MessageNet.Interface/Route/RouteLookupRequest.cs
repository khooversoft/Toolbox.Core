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
