// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Azure;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    public interface IMessageRepository
    {
        Task<QueueReceiver<NetMessageModel>> Register(QueueId queueId, CancellationToken token);
        Task<IReadOnlyList<QueueDefinition>> Search(string search, CancellationToken token);
        Task Unregister(QueueId queueId, CancellationToken token);
    }
}