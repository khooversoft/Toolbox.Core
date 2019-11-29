// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public interface IActorManager : IDisposable
    {
        IActorManager Register<T>(Func<IWorkContext, T> createImplementation) where T : IActor;

        Task<T> CreateProxy<T>(string actorKey) where T : IActor;

        Task<T> CreateProxy<T>(ActorKey actorKey) where T : IActor;

        Task<bool> Deactivate<T>(ActorKey actorKey);

        Task<bool> Deactivate(Type actorType, ActorKey actorKey);

        Task DeactivateAll();

        ActorConfiguration Configuration { get; }
    }
}
