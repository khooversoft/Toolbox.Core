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
        public NodeRegistrationModel(string nodeId, string inputUri)
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
