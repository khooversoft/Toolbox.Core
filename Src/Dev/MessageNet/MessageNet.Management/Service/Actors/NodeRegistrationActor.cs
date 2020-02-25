// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Management
{
    public class NodeRegistrationActor : ActorBase, INodeRegistrationActor
    {
        private readonly IMessageNetConfig _config;
        private readonly IRegisterStore _registerStore;
        private readonly CacheObject<QueueId> _cache = new CacheObject<QueueId>(TimeSpan.FromMinutes(10));
        private readonly QueueId _queueId;

        public NodeRegistrationActor(IMessageNetConfig config, IRegisterStore registerStore)
        {
            config.Verify(nameof(config)).IsNotNull();
            registerStore.Verify(nameof(registerStore)).IsNotNull();

            _config = config;
            _registerStore = registerStore;
            _queueId = QueueId.Parse(ActorKey.VectorKey);
        }

        public async Task<QueueId> Set(IWorkContext context)
        {
            context.Telemetry.Verbose(context, $"Set node registration {ActorKey.VectorKey}");

            var queueId = new QueueId(_config.Namespace, _queueId.NetworkId, _queueId.NodeId);
            await _registerStore.Set(context, ActorKey.VectorKey, queueId);

            _cache.Set(queueId);
            return queueId;
        }

        public async Task<QueueId?> Get(IWorkContext context)
        {
            if (_cache.TryGetValue(out QueueId model)) return model;

            QueueId? subject = await _registerStore.Get(context, ActorKey.VectorKey);
            if (subject == null) return null;

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
    }
}
