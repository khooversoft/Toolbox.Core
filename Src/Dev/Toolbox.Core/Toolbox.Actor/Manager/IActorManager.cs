// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public interface IActorManager : IDisposable
    {
        IActorManager Register<T>(IWorkContext context, Func<IWorkContext, T> createImplementation) where T : IActor;

        Task<T> CreateProxy<T>(IWorkContext context, string actorKey) where T : IActor;

        Task<T> CreateProxy<T>(IWorkContext context, ActorKey actorKey) where T : IActor;

        Task<bool> Deactivate<T>(IWorkContext context, ActorKey actorKey);

        Task<bool> Deactivate(IWorkContext context, Type actorType, ActorKey actorKey);

        Task DeactivateAll(IWorkContext context);

        IActorConfiguration Configuration { get; }
    }
}
