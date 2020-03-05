// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using System;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    public interface IMessageAwaiterManager
    {
        void Add(Guid id, TaskCompletionSource<NetMessage> task);

        void SetResult(NetMessage? netMessage);
    }
}