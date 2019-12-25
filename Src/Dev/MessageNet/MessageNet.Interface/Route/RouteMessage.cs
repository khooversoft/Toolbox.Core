using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageHub.Interface
{
    /// <summary>
    /// Route message envelope
    /// </summary>
    public class RouteMessage
    {
        /// <summary>
        /// From node URI (required)
        /// </summary>
        public string? FromUri { get; set; }

        /// <summary>
        /// To node URI (required)
        /// </summary>
        public string? ToUri { get; set; }

        /// <summary>
        /// Date sent UTC
        /// </summary>
        public DateTime DateSentUtc { get; set; }

        /// <summary>
        /// Message type (not required)
        /// </summary>
        public string? MessageType { get; set; }
    }

    public class RouteMessage<T> : RouteMessage
        where T : class
    {
        public T? Message { get; set; }
    }
}
