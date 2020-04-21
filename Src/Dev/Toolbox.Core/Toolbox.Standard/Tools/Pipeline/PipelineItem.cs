// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public struct PipelineItem<T>
    {
        public PipelineItem(Func<T, bool> predicate, Func<IWorkContext, T, Func<IWorkContext, T, Task>, Task> middleware)
        {
            predicate.VerifyNotNull(nameof(predicate));
            middleware.VerifyNotNull(nameof(middleware));

            Predicate = predicate;
            Middleware = middleware;
        }

        public Func<T, bool> Predicate { get; }

        public Func<IWorkContext, T, Func<IWorkContext, T, Task>, Task> Middleware { get; }

        public Task Invoke(IWorkContext context, T message, Func<IWorkContext, T, Task> next)
        {
            if (!Predicate(message)) return next(context, message);

            return Middleware(context, message, next);
        }
    }
}
