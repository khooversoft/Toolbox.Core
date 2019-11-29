// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public interface IActorBase : IDisposable
    {
        ActorKey ActorKey { get; }

        bool Active { get; }

        Task Activate(IWorkContext context);

        Task Deactivate(IWorkContext context);
    }
}
