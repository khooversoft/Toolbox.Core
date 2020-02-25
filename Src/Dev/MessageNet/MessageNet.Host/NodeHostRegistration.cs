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
        public NodeHostRegistration(MessageUri messageUri, Func<NetMessage, Task> receiver)
        {
            messageUri.Verify(nameof(messageUri)).IsNotNull();
            receiver.Verify(nameof(receiver)).IsNotNull();

            MessageUri = messageUri;
            Receiver = receiver;
        }

        public MessageUri MessageUri { get; }

        public Func<NetMessage, Task> Receiver { get; }
    }
}
