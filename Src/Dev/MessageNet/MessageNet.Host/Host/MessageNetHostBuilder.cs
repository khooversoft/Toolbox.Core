// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Host
{
    public class MessageNetHostBuilder
    {
        public IMessageNetConfig? MessageNetConfig { get; set; }

        public IList<NodeHostRegistration> NodeRegistrations { get; } = new List<NodeHostRegistration>();

        public MessageNetHostBuilder SetMessageNetConfig(IMessageNetConfig messageNetConfig)
        {
            MessageNetConfig = messageNetConfig;
            return this;
        }

        public MessageNetHostBuilder AddNodeRegistrations(params NodeHostRegistration[] nodeHostRegistrations)
        {
            nodeHostRegistrations.ForEach(x => NodeRegistrations.Add(x));
            return this;
        }

        public IMessageNetHost Build(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            MessageNetConfig.Verify(nameof(MessageNetConfig)).IsNotNull();
            NodeRegistrations.Verify(nameof(NodeRegistrations)).IsNotNull().Assert(x => x!.Count > 0, "Node registrations are required");

            return new MessageNetHost(context, MessageNetConfig!, NodeRegistrations!);
        }
    }
}
