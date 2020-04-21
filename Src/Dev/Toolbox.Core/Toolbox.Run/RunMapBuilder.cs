using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Graph;

namespace Khooversoft.Toolbox.Run
{
    public class RunMapBuilder : IEnumerable<IActivityCommon>
    {
        private readonly IList<IActivityCommon> _list = new List<IActivityCommon>();
        private bool _strict = false;
        private string? _nameSpace;

        public RunMapBuilder() { }

        public RunMapBuilder SetStrict(bool strict) { _strict = strict; return this; }

        public RunMapBuilder SetNamespace(string nameSpace) { _nameSpace = nameSpace; return this; }

        public void Add(IActivityCommon activityCommon) => _list.Add(activityCommon.VerifyNotNull(nameof(activityCommon)));

        public RunMap Build()
        {
            IReadOnlyList<IReadOnlyList<IActivityCommon>> sequences = BuildSequences();

            var runMap = new RunMap(_strict);

            // Add all activities and add to map
            sequences
                .SelectMany(x => x)
                .OfType<IActivity>()
                .Where(x => !runMap.Nodes.ContainsKey(x.Key))
                .ForEach(x => runMap.Add(x));

            sequences
                .SelectMany(x => x)
                .OfType<ControlFlow>()
                .Where(x => x.ToActivity != null && !runMap.Nodes.ContainsKey(x.ToActivity.Name))
                .ForEach(x => runMap.Add(x.ToActivity!));

            // Process control flow in sequence
            foreach (var sequence in sequences)
            {
                Cursor<IActivityCommon> cursor = new Cursor<IActivityCommon>(sequence);
                while (cursor.TryNextValue(out IActivityCommon? item))
                {
                    switch (item)
                    {
                        case ControlFlow controlFlow:
                            IActivity fromActivity = sequence.Take(cursor.Index)
                                .OfType<IActivity>()
                                .Reverse()
                                .FirstOrDefault()
                                .VerifyNotNull($"Cannot find Activity for control flow at {cursor.Index}");

                            Func<IActivity> getNextActivity = () => sequence.Skip(cursor.Index + 1)
                                .OfType<IActivity>()
                                .FirstOrDefault()
                                .VerifyNotNull($"After control flow at {cursor.Index} is not an activity")!;

                            IActivity toActivity = controlFlow.ToActivity ?? getNextActivity();

                            runMap.Add(controlFlow.PredicateAsync != null
                                ? new ActivityControlFlow(fromActivity.Name, toActivity.Name, controlFlow.PredicateAsync)
                                : new ActivityControlFlow(fromActivity.Name, toActivity.Name, controlFlow.Predicate!)
                                );
                            break;
                    }
                }
            }

            return runMap;
        }

        private IReadOnlyList<IReadOnlyList<IActivityCommon>> BuildSequences()
        {
            IList<IReadOnlyList<IActivityCommon>> result = new List<IReadOnlyList<IActivityCommon>>();
            var sequencesToRun = new Stack<(string? nameSpace, IEnumerable<IActivityCommon>)>();
            var currentSequence = new List<IActivityCommon>();

            sequencesToRun.Push((_nameSpace, _list));

            while (sequencesToRun.Count > 0)
            {
                (string? ns, IEnumerable<IActivityCommon> processList) = sequencesToRun.Pop();
                StringVector nameSpace = ns.IsEmpty() ? StringVector.EmptyNoRoot : new StringVector(ns!);
                currentSequence = new List<IActivityCommon>();

                foreach (var item in processList)
                {
                    switch (item)
                    {
                        case Activity activity:
                            currentSequence.Add(activity.WithName(nameSpace + activity.Name));
                            break;

                        case ActivityControlFlow flow:
                            currentSequence.Add(flow.WithName(nameSpace + flow.FromActivityName, nameSpace + flow.ToActivityName));
                            break;

                        case ControlFlow controlFlow:
                            currentSequence.Add(controlFlow);
                            break;

                        case IActivity _:
                        case IControlFlow _:
                            currentSequence.Add(item);
                            break;

                        case Sequence runSequence:
                            sequencesToRun.Push((runSequence.Namespace, runSequence));
                            break;

                        default:
                            throw new ArgumentException($"Invalid element {item.GetType().Name}");
                    }
                }

                result.Add(currentSequence);
            }

            return (IReadOnlyList<IReadOnlyList<IActivityCommon>>)result;
        }

        public IEnumerator<IActivityCommon> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
