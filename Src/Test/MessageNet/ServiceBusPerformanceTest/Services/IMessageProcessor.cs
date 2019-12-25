// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;

namespace ServiceBusPerformanceTest
{
    internal interface IMessageProcessor
    {
        Task Close();

        Task Register(IWorkContext context, Func<Message, Task> receiver);
    }
}