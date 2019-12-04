// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Graph
{
    public interface IJob
    {
        /// <summary>
        /// True if job is active, false if not, still marks node as being executed
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Triggered when the application host is ready to start the job.
        /// </summary>
        /// <param name="context">context</param>
        Task Start(IWorkContext context);

        /// <summary>
        /// Triggered when the application job is performing a graceful shutdown.
        /// </summary>
        /// <param name="context">context</param>
        Task Stop(IWorkContext context);

        /// <summary>
        /// Get job results
        /// </summary>
        /// <returns>results</returns>
        IJobResult GetResult(IWorkContext context);
    }
}
