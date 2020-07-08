using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Run
{
    public class RunMap : GraphMap<string, IActivity, IControlFlow>
    {
        public RunMap(bool strict = true)
            : base(strict)
        {
        }

        public void Add(IActivityCommon subject)
        {
            switch (subject)
            {
                case IActivity activity:
                    base.Add(activity);
                    break;

                case IControlFlow edge:
                    base.Add(edge);
                    break;
            }
        }

        public Task Send<T>(T message, string? name = null, IRunContext? runContext = null)
        {
            message.VerifyNotNull(nameof(message));
            return Run(new RunContext().SetMessage<T>(message), name);
        }

        public async Task Run(IRunContext? runContext = null, string? name = null)
        {
            runContext ??= new RunContext();

            IReadOnlyList<string> startActivities = name != null
                ? new string[] { name }
                : this.TopologicalSort().FirstOrDefault()?.Select(x => x.Name)?.ToArray() ?? Array.Empty<string>();

            var runningList = new List<TrackActivity>();

            var newActivities = startActivities
                .Select(x => new TrackActivity(Nodes[x], Nodes[x].Run(runContext)))
                .ToList();

            runningList.AddRange(newActivities);

            while (runningList.Count != 0)
            {
                int taskNumber = Task.WaitAny(runningList.Select(x => x.ActivityTask).ToArray());

                TrackActivity finished = runningList[taskNumber];
                runningList.RemoveAt(taskNumber);

                var edges = GetEdgesForNode(finished.Activity);
                foreach (var edge in edges)
                {
                    bool pass = await edge.IsValid(runContext, finished.Activity);
                    if (pass)
                    {
                        IActivity activity = Nodes[edge.ToActivityName];
                        runningList.Add(new TrackActivity(activity, activity.Run(runContext)));
                    }
                }
            }
        }

        private struct TrackActivity
        {
            public TrackActivity(IActivity activity, Task activityTask)
            {
                Activity = activity;
                ActivityTask = activityTask;
            }

            public IActivity Activity { get; }

            public Task ActivityTask { get; }
        }
    }
}
