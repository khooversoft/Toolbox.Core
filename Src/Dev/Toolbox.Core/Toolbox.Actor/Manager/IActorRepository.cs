// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public interface IActorRepository
    {
        Task Clear(IWorkContext context);

        Task Set(IWorkContext context, IActorRegistration registration);

        IActorRegistration? Lookup(Type actorType, ActorKey actorKey);

        Task<IActorRegistration?> Remove(IWorkContext context, Type actorType, ActorKey actorKey);
    }
}
