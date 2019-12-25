// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageHub.Management
{
    /// <summary>
    /// Verify queue existence and/or remove it.
    /// </summary>
    public class RemoveQueueState : IStateItem
    {
        private readonly IQueueManagement _managementClient;

        public RemoveQueueState(IQueueManagement queueManagement, string nodeId)
        {
            queueManagement.Verify(nameof(queueManagement)).IsNotNull();
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();

            _managementClient = queueManagement;
            Name = nodeId;
        }

        public string Name { get; }

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            if (await Test(context)) return true;

            await _managementClient.DeleteQueue(context, Name);
            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            bool state = await _managementClient.QueueExists(context, Name);
            return !state;
        }
    }
}
