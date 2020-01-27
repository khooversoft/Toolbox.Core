// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class NodeRegistrationModel
    {
        public NodeRegistrationModel(string netorkId, string nodeId, string inputQueueUri)
        {
            netorkId.Verify(nameof(netorkId)).IsNotEmpty();
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();
            inputQueueUri.Verify(nameof(inputQueueUri)).IsNotEmpty();

            NetorkId = netorkId;
            NodeId = nodeId;
            InputQueueUri = inputQueueUri;
        }

        public string NetorkId { get; }

        public string NodeId { get; }

        public string InputQueueUri { get; }
    }
}
