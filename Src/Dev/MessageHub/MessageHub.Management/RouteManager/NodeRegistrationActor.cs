// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageHub.Management
{
    public class NodeRegistrationActor : ActorBase, INodeRegistrationActor
    {
        private readonly IRegisterStore _registerStore;
        private readonly CacheObject<NodeRegistrationModel> _cache = new CacheObject<NodeRegistrationModel>(TimeSpan.FromMinutes(10));

        public NodeRegistrationActor(IRegisterStore registerStore)
        {
            registerStore.Verify(nameof(registerStore)).IsNotNull();

            _registerStore = registerStore;
        }

        public async Task<NodeRegistrationModel?> Get(IWorkContext context)
        {
            if (_cache.TryGetValue(out NodeRegistrationModel model)) return model;

            NodeRegistrationModel? subject = await _registerStore.Get(context, ActorKey.VectorKey);
            if (subject == null) return subject;

            _cache.Set(subject);
            return subject;
        }

        public Task Remove(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            context.Telemetry.Verbose(context, $"Removing node registration {ActorKey.VectorKey}");

            _cache.Clear();
            return _registerStore.Remove(context, ActorKey.VectorKey);
        }

        public async Task Set(IWorkContext context, NodeRegistrationModel nodeRegistrationModel)
        {
            context.Telemetry.Verbose(context, $"Set node registration {ActorKey.VectorKey}");

            await _registerStore.Set(context, ActorKey.VectorKey, nodeRegistrationModel);
            _cache.Set(nodeRegistrationModel);
        }
    }
}
