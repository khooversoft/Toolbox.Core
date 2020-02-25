// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class NodeRegistration
    {
        public NodeRegistration(string nameSpace, QueueId queueId)
        {
            nameSpace.Verify(nameof(nameSpace)).IsNotNull();
            queueId.Verify(nameof(queueId)).IsNotNull();

            Namespace = nameSpace;
            QueueId = queueId;
        }

        public string Namespace { get; }

        public QueueId QueueId { get; }
    }
}
