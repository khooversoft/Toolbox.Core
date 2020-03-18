// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    public interface IMessageNetHost : IDisposable
    {
        Task Start(IWorkContext context);

        Task Stop(IWorkContext context);

        Task<IMessageClient> GetMessageClient(IWorkContext context, QueueId queueId);

        Task Send(IWorkContext context, NetMessage netMessage);

        Task<NetMessage> Call(IWorkContext context, NetMessage netMessage, TimeSpan? timeout = null);
    }
}
