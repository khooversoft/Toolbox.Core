// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Graph
{
    public class JobHost : IJobHost
    {
        private readonly Dictionary<Guid, JobEntry> _currentJob = new Dictionary<Guid, JobEntry>();
        private readonly object _lock = new object();

        public JobHost()
        {
        }

        public int JobsRunning
        {
            get
            {
                lock (_lock)
                {
                    return _currentJob.Count;
                }
            }
        }

        public Task<Guid> StartJob(IWorkContext context, IJob job, Action<IWorkContext, IJobResult> completionNotification)
        {
            job.VerifyNotNull(nameof(job));
            completionNotification.VerifyNotNull(nameof(completionNotification));

            lock (_lock)
            {
                var jobEntry = new JobEntry(job, completionNotification);

                _currentJob.Add(jobEntry.JobId, jobEntry);

                context.Telemetry.Verbose(context, $"Starting Job Id={jobEntry.JobId}");

                jobEntry.RunningTask = Task.Run(async () => await job.Start(context))
                    .ContinueWith(x => Completed(context, x, jobEntry.JobId, job.GetResult(context)));

                context.Telemetry.Verbose(context, $"Started Job Id={jobEntry.JobId}");
                return Task.FromResult(jobEntry.JobId);
            }
        }

        public async Task<bool> StopJob(IWorkContext context, Guid jobId)
        {
            JobEntry jobEntry = default!;

            lock (_lock)
            {
                if (!_currentJob.TryGetValue(jobId, out jobEntry))
                {
                    return false;
                }

                Verify.Assert<InvalidOperationException>(_currentJob.Remove(jobId), $"Failed to remove job id {jobId} from collection");
            }

            if (jobEntry.RunningTask?.IsCompleted == false)
            {
                await jobEntry.Job.Stop(context);
            }

            return true;
        }

        private void Completed(IWorkContext context, Task task, Guid jobId, IJobResult jobResult)
        {
            JobEntry jobEntry = null!;

            lock (_lock)
            {
                if (!_currentJob.TryGetValue(jobId, out jobEntry))
                {
                    return;
                }

                context.Telemetry.Verbose(context, $"Removed {jobId} from current job list because signaled completed");
                _currentJob.Remove(jobId);
            }

            JobStatus status = task.IsFaulted ? JobStatus.Faulted : jobResult.Status;

            TimeSpan duration = DateTimeOffset.Now - jobEntry.StartTime;
            string msg = $"Job completed - Job Id={jobId}, Status={jobResult.Status}, IsFaulted={task.IsFaulted}, Duration={duration}";

            if (!task.IsFaulted && jobResult.Status == JobStatus.Completed)
            {
                context.Telemetry.Verbose(context, msg);
            }
            else
            {
                context.Telemetry.Error(context, msg, task.Exception);
            }

            jobEntry?.CompletionNotification(context, new JobResult(jobId, status, duration, jobResult.Errors, task.Exception));

            if (task.IsFaulted)
            {
                context.Telemetry.Verbose(context, "Task disposed");
                task.Dispose();
            }
        }

        public bool WaitAny(IWorkContext context)
        {
            Task[] list;

            lock (_lock)
            {
                list = _currentJob.Values
                    .Select(x => x.RunningTask!)
                    .ToArray();
            }

            if (list.Length == 0)
            {
                return false;
            }

            int index = Task.WaitAny(list, context.CancellationToken);
            return true;
        }


        public bool WaitAll(IWorkContext context, TimeSpan? timeout = null)
        {
            Task[] list;

            lock (_lock)
            {
                list = _currentJob.Values
                    .Select(x => x.RunningTask!)
                    .ToArray();
            }

            if (list.Length == 0)
            {
                return false;
            }

            timeout = timeout ?? TimeSpan.FromMilliseconds(-1);

            Task.WaitAll(list, (int)((TimeSpan)timeout).TotalMilliseconds, context.CancellationToken);
            return true;
        }


        private class JobEntry
        {
            public JobEntry(IJob job, Action<IWorkContext, IJobResult> completionNotification)
            {
                JobId = Guid.NewGuid();

                Job = job;
                CompletionNotification = completionNotification;
            }

            public Guid JobId { get; }

            public IJob Job { get; }

            public Action<IWorkContext, IJobResult> CompletionNotification { get; }

            public Task? RunningTask { get; set; }

            public DateTimeOffset StartTime = DateTimeOffset.Now;
        }
    }
}
