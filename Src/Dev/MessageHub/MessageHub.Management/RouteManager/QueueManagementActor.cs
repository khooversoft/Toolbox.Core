// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageHub.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageHub.Management
{
    public class QueueManagementActor : ActorBase, IQueueManagementActor
    {
        private readonly IQueueManagement _queueManagement;
        private readonly CacheObject<QueueDefinition> _cache = new CacheObject<QueueDefinition>(TimeSpan.FromMinutes(10));

        public QueueManagementActor(IQueueManagement queueMangement)
        {
            queueMangement.Verify(nameof(queueMangement)).IsNotNull();

            _queueManagement = queueMangement;
        }

        public async Task<QueueDefinition?> Get(IWorkContext context)
        {
            if (_cache.TryGetValue(out QueueDefinition model)) return model;

            bool exist = await _queueManagement.QueueExists(context, ActorKey.VectorKey);
            if (!exist) return null;

            QueueDefinition subject = await _queueManagement.GetQueue(context, ActorKey.VectorKey);
            _cache.Set(subject);

            return subject;
        }

        public Task Remove(IWorkContext context)
        {
            context.Telemetry.Verbose(context, $"Removing queue {ActorKey.VectorKey}");

            _cache.Clear();

            return new StateManagerBuilder()
                .Add(new RemoveQueueState(_queueManagement, ActorKey.VectorKey))
                .Build()
                .Set(context);
        }

        public async Task Set(IWorkContext context, QueueDefinition queueDefinition)
        {
            context.Telemetry.Verbose(context, $"Set queue {ActorKey.VectorKey}");

            bool state = await new StateManagerBuilder()
                .Add(new CreateQueueState(_queueManagement, queueDefinition))
                .Build()
                .Set(context);

            if (!state)
            {
                context.Telemetry.Error(context, $"Failed to create/set queue {ActorKey.VectorKey}");
                throw new InvalidOperationException($"Failed to create queue {queueDefinition.QueueName}");
            }

            _cache.Set(queueDefinition);
        }

    }
}
