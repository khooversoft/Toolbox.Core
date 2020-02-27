// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageNet.Host
{
    public interface IRouteRepository
    {
        Task<QueueReceiver<NetMessage>> Register(IWorkContext context, QueueId queueId);

        Task Unregister(IWorkContext context, QueueId queueId);

        Task<IReadOnlyList<QueueDefinition>> Search(IWorkContext context, string search);
    }
}