// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    public interface IMessageProcessor
    {
        Task Stop();

        void Dispose();

        Task Start(IWorkContext context, Func<NetMessage, Task> receiver);
    }
}