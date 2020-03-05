﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    internal class MessageAwaiterManager : IMessageAwaiterManager
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<NetMessage>> _completion = new ConcurrentDictionary<Guid, TaskCompletionSource<NetMessage>>();

        public MessageAwaiterManager() { }

        public void Add(Guid id, TaskCompletionSource<NetMessage> task)
        {
            task.Verify(nameof(task)).IsNotNull();

            _completion[id] = task;
        }

        public void SetResult(NetMessage? netMessage)
        {
            if (netMessage == null!) return;

            TaskCompletionSource<NetMessage> tcs;

            if (_completion.TryRemove(netMessage.Header.MessageId, out tcs!))
            {
                tcs.SetResult(netMessage);
            }
        }
    }
}
