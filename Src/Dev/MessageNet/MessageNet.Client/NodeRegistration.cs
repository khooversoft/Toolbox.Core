using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Client
{
    internal class NodeRegistration
    {
        public NodeRegistration(string nodeId, string inputUri)
        {
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();
            inputUri.Verify(nameof(inputUri)).IsNotEmpty();

            NodeId = nodeId;
            InputUri = inputUri;
        }

        public string NodeId { get; }

        public string InputUri { get; }
    }
}
