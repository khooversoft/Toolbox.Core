using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class NodeRegistrationStoreModel
    {
        /// <summary>
        /// Message name space
        /// </summary>
        public string? Namespace { get; set; }

        /// <summary>
        /// Network ID
        /// </summary>
        public string? NetworkId { get; set; }

        /// <summary>
        /// Network ID
        /// </summary>
        public string? NodeId { get; set; }
    }
}
