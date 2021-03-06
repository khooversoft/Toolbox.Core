﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public interface IActorRegistration : IDisposable
    {
        Type ActorType { get; }

        ActorKey ActorKey { get; }

        IActorBase Instance { get; }

        T GetInstance<T>() where T : IActor;
    }
}
