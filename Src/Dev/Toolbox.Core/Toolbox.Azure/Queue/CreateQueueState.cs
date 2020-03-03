// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    /// <summary>
    /// Verify queue does exist or create it
    /// </summary>
    public class CreateQueueState : IStateItem
    {
        private readonly QueueDefinition _queueDefinition;
        private readonly IQueueManagement _managementClient;

        public CreateQueueState(IQueueManagement queueManagement, QueueDefinition queueDefinition)
        {
            queueManagement.Verify(nameof(queueManagement)).IsNotNull();
            queueDefinition.Verify(nameof(queueDefinition)).IsNotNull();
            queueDefinition.QueueName.Verify(nameof(queueDefinition.QueueName)).IsNotNull();

            _queueDefinition = queueDefinition;
            _managementClient = queueManagement;
        }

        public string Name => _queueDefinition!.QueueName!;

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            if (await Test(context)) return true;

            await _managementClient.CreateQueue(context, _queueDefinition);
            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            bool state = await _managementClient.QueueExists(context, Name);
            return state;
        }
    }
}
