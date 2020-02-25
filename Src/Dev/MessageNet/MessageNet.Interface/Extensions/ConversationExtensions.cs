// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class ConversationExtensions
    {
        public static NodeRegistration ConvertTo(this RouteResponse subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new NodeRegistration(subject.Namespace!, new QueueId(subject.NetworkId!, subject.NodeId!));
        }
    }
}
