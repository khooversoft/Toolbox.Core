// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    public class NodeHostRegistration
    {
        public NodeHostRegistration(QueueId queueId, Func<NetMessage, Task> receiver)
        {
            queueId.Verify(nameof(queueId)).IsNotNull();
            receiver.Verify(nameof(receiver)).IsNotNull();

            QueueId = queueId;
            Receiver = receiver;
        }

        public QueueId QueueId { get; }

        public Func<NetMessage, Task> Receiver { get; }
    }
}
