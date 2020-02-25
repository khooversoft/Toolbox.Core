// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    internal class NodeRouteActor : ActorBase, INodeRouteActor
    {
        private readonly INameServerClient _nameServer;
        private readonly CacheObject<NodeRegistration> _cache = new CacheObject<NodeRegistration>(TimeSpan.FromHours(1));
        private readonly QueueId _queueId;

        public NodeRouteActor(INameServerClient nameServer)
        {
            _nameServer = nameServer;
            _queueId = QueueId.Parse(ActorKey.VectorKey);
        }

        public async Task<NodeRegistration?> Lookup(IWorkContext context)
        {
            if (_cache.TryGetValue(out NodeRegistration cachedValue)) return cachedValue;

            RouteResponse? lookup = await _nameServer.Lookup(context, new RouteRequest { NetworkId = _queueId.NetworkId, NodeId = _queueId.NodeId });
            if (lookup == null) return null;

            NodeRegistration lookupModel = lookup.ConvertTo();

            _cache.Set(lookupModel);
            return lookupModel;
        }

        public async Task<NodeRegistration> Register(IWorkContext context)
        {
            _cache.Clear();

            var request = new RouteRequest { NetworkId = _queueId.NetworkId, NodeId = _queueId.NodeId };

            RouteResponse response = await _nameServer.Register(context, request);
            response.Verify().IsNotNull("Registration failed with name server");

            NodeRegistration nodeRegistrationModel = response.ConvertTo();
            _cache.Set(nodeRegistrationModel);

            return nodeRegistrationModel;
        }
    }
}
