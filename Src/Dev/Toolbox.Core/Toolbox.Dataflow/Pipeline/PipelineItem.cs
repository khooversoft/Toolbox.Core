// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Dataflow
{
    public struct PipelineItem<T>
    {
        public PipelineItem(Func<T, bool> predicate, Func<T, Func<T, Task>, Task> middleware)
        {
            predicate.VerifyNotNull(nameof(predicate));
            middleware.VerifyNotNull(nameof(middleware));

            Predicate = predicate;
            Middleware = middleware;
        }

        public Func<T, bool> Predicate { get; }

        public Func<T, Func<T, Task>, Task> Middleware { get; }

        public Task Invoke(T message, Func<T, Task> next)
        {
            if (!Predicate(message)) return next(message);

            return Middleware(message, next);
        }
    }
}
