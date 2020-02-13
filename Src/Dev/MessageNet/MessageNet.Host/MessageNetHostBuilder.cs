﻿// Copyright (c) KhooverSoft. All rights reserved.
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
        public Uri? NameServerUri { get; set; }

        public IConnectionManager? ConnectionManager { get; set; }

        public IList<NodeHostRegistration>? NodeRegistrations { get; set; }

        public MessageNetHostBuilder SetNameServerUri(Uri nameServerUri)
        {
            NameServerUri = nameServerUri;
            return this;
        }

        public MessageNetHostBuilder UseConnectionManager(IConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
            return this;
        }

        public MessageNetHostBuilder SetNodeRegistration(params NodeHostRegistration[] nodeRegistrations)
        {
            NodeRegistrations = nodeRegistrations.ToList();
            return this;
        }

        public IMessageNetHost Build(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            NameServerUri.Verify(nameof(NameServerUri)).IsNotNull();
            ConnectionManager.Verify(nameof(ConnectionManager)).IsNotNull();
            NodeRegistrations.Verify(nameof(NodeRegistrations)).IsNotNull().Assert(x => x!.Count > 0, "Node registrations are required");

            return new MessageNetHost(context, NameServerUri!, ConnectionManager!, NodeRegistrations!);
        }
    }
}
