// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageHub.Management
{
    public interface IQueueManagementActor : IActor
    {
        Task<QueueDefinition?> Get(IWorkContext context);

        Task Remove(IWorkContext context);

        Task Set(IWorkContext context, QueueDefinition queueDefinition);
    }
}