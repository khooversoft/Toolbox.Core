// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Graph
{
    public interface IJobHost
    {
        int JobsRunning { get; }

        Task<Guid> StartJob(IWorkContext context, IJob job, Action<IWorkContext, IJobResult> completionNotification);

        Task<bool> StopJob(IWorkContext context, Guid jobId);

        bool WaitAny(IWorkContext context);

        bool WaitAll(IWorkContext context, TimeSpan? timeout = null);
    }
}
