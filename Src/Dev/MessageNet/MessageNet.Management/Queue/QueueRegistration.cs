// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Management
{
    public class QueueRegistration
    {

        public QueueRegistration(string serviceHubNamespace, string nodeId, QueueDefinition queueDefinition)
        {
            serviceHubNamespace.Verify(nameof(serviceHubNamespace)).IsNotEmpty();
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();
            queueDefinition.Verify(nameof(queueDefinition)).IsNotNull();

            ServiceHubNamespace = serviceHubNamespace;
            NodeId = nodeId;
            QueueDefinition = queueDefinition;
        }

        public string ServiceHubNamespace { get; }

        public string NodeId { get; }

        public QueueDefinition QueueDefinition { get; }
    }
}
