// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public class JobResult : IJobResult
    {
        public JobResult(JobStatus status, IEnumerable<string>? errors = null, AggregateException? exception = null)
        {
            Status = status;
            Errors = errors?.ToList();
            Exception = exception;
        }

        public JobResult(Guid jobId, JobStatus status, TimeSpan duration, IEnumerable<string>? errors = null, AggregateException? exception = null)
            : this(status, errors, exception)
        {
            JobId = jobId;
            Duration = duration;
        }

        public Guid? JobId { get; }

        public JobStatus Status { get; }

        public AggregateException? Exception { get; }

        public IReadOnlyList<string>? Errors { get; }

        public TimeSpan? Duration { get; }
    }
}
