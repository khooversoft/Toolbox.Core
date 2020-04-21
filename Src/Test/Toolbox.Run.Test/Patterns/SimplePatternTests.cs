using FluentAssertions;
using Khooversoft.Toolbox.Run;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Run.Test.Patterns
{
    public class SimplePatternTests
    {
        private static readonly Func<Action, Task> voidTask = async f => { f(); await Task.Delay(TimeSpan.FromMilliseconds(100)); };
        private static readonly Func<Action, Task<bool>> trueTask = async f => { f(); await Task.Delay(TimeSpan.FromMilliseconds(200)); return true; };

        [Fact]
        public async Task TestSequence_TwoActivities()
        {
            var runMap = new RunMap()
            {
                new Activity("Start", (c, r) => voidTask(() => r.Activity.Properties.SetSuccess())),

                new Activity("Setup", (c, r) => voidTask(() => r.Activity.Properties.SetSuccess())),
                new ActivityControlFlow("start", "Setup", (c, r) => Task.FromResult(r.Activity.Properties.IsSuccess())),

                new Activity("Execute", (c, r) => voidTask(() => r.Activity.Properties.SetSuccess())),
                new ActivityControlFlow("Setup", "Execute", (c, r) => Task.FromResult(r.Activity.Properties.IsSuccess())),
            };

            await runMap.Run(WorkContextBuilder.Default, new RunContext());

            foreach (IActivity activity in runMap.Nodes.Values)
            {
                activity.Properties.IsSuccess().Should().BeTrue();
            }
        }

        [Fact]
        public async Task TestSequence_ThreeWithTwoInParallelActivities()
        {
            var queue = new ConcurrentQueue<(string node, int threadId)>();

            var runMap = new RunMap()
            {
                new Activity("Start", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId)))),

                new Activity("PlanA", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId)))),
                new ActivityControlFlow("start", "PlanA", (c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Start-PlanA", Thread.CurrentThread.ManagedThreadId)))),

                new Activity("PlanB", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId)))),
                new ActivityControlFlow("Start", "PlanB", (c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Start-PlanB", Thread.CurrentThread.ManagedThreadId)))),
            };

            await runMap.Run(WorkContextBuilder.Default, new RunContext());

            var requiredSequence = new[]
            {
                "Start",
                "Start-Start-PlanA",
                "PlanA",
                "Start-Start-PlanB",
                "PlanB",
            };

            Enumerable.SequenceEqual(requiredSequence, queue.Select(x => x.node)).Should().BeTrue();
        }

        [Fact]
        public async Task TestSequenceShortForm_TwoSerialActivities()
        {
            var queue = new ConcurrentQueue<(string node, int threadId)>();

            var start = new Activity("Start", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));
            var plana = new Activity("PlanA", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));
            var planb = new Activity("PlanB", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));

            var builder = new RunMapBuilder()
            {
                start,
                new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Start-PlanA", Thread.CurrentThread.ManagedThreadId)))),
                plana,
                new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Start-PlanB", Thread.CurrentThread.ManagedThreadId)))),
                planb,
            };

            RunMap runMap = builder.Build();

            runMap.Nodes.Count.Should().Be(3);
            runMap.Nodes.Values.Where(x => x.Name == "Start").Count().Should().Be(1);
            runMap.Nodes.Values.Where(x => x.Name == "PlanA").Count().Should().Be(1);
            runMap.Nodes.Values.Where(x => x.Name == "PlanB").Count().Should().Be(1);

            runMap.Edges.Count.Should().Be(2);
            runMap.Edges.Values.Where(x => x.FromActivityName == "Start" && x.ToActivityName == "PlanA").Count().Should().Be(1);
            runMap.Edges.Values.Where(x => x.FromActivityName == "PlanA" && x.ToActivityName == "PlanB").Count().Should().Be(1);

            await runMap.Run(WorkContextBuilder.Default, new RunContext());

            var requiredSequence = new[]
{
                "Start",
                "Start-Start-PlanA",
                "PlanA",
                "PlanA-Start-PlanB",
                "PlanB",
            };

            Enumerable.SequenceEqual(requiredSequence, queue.Select(x => x.node)).Should().BeTrue();
        }

        [Fact]
        public async Task TestSequenceShortForm_TwoSerialActivitiesWithSequence()
        {
            var queue = new ConcurrentQueue<(string node, int threadId)>();

            var start = new Activity("Start", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));
            var plana = new Activity("PlanA", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));
            var planb = new Activity("PlanB", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));

            var builder = new RunMapBuilder()
            {
                new Sequence()
                    + start
                    + new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Start-PlanA", Thread.CurrentThread.ManagedThreadId))))
                    + plana
                    + new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Start-PlanB", Thread.CurrentThread.ManagedThreadId))))
                    + planb
            };

            RunMap runMap = builder.Build();

            runMap.Nodes.Count.Should().Be(3);
            runMap.Nodes.Values.Where(x => x.Name == "Start").Count().Should().Be(1);
            runMap.Nodes.Values.Where(x => x.Name == "PlanA").Count().Should().Be(1);
            runMap.Nodes.Values.Where(x => x.Name == "PlanB").Count().Should().Be(1);

            runMap.Edges.Count.Should().Be(2);
            runMap.Edges.Values.Where(x => x.FromActivityName == "Start" && x.ToActivityName == "PlanA").Count().Should().Be(1);
            runMap.Edges.Values.Where(x => x.FromActivityName == "PlanA" && x.ToActivityName == "PlanB").Count().Should().Be(1);

            await runMap.Run(WorkContextBuilder.Default, new RunContext());

            var requiredSequence = new[]
{
                "Start",
                "Start-Start-PlanA",
                "PlanA",
                "PlanA-Start-PlanB",
                "PlanB",
            };

            Enumerable.SequenceEqual(requiredSequence, queue.Select(x => x.node)).Should().BeTrue();
        }

        [Fact]
        public async Task TestSequenceShortForm_TwoSerialSequences()
        {
            var queue = new ConcurrentQueue<(string node, int threadId)>();

            var start = new Activity("Start", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));
            var plana = new Activity("PlanA", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));
            var planb = new Activity("PlanB", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));
            var planc = new Activity("PlanC", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));

            var builder = new RunMapBuilder()
            {
                new Sequence("S1")
                    + start
                    + new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Link-PlanA(1)", Thread.CurrentThread.ManagedThreadId))))
                    + plana
                    + new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Link-PlanB", Thread.CurrentThread.ManagedThreadId))))
                    + planb,

                new Sequence("S2")
                    + start
                    + new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Link-PlanA(2)", Thread.CurrentThread.ManagedThreadId))))
                    + plana
                    + new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Link-PlanC", Thread.CurrentThread.ManagedThreadId))))
                    + planc
            };

            RunMap runMap = builder.Build();

            var nodes = new[]
            {
                "S1/Start",
                "S1/PlanA",
                "S1/PlanB",
                "S2/Start",
                "S2/PlanA",
                "S2/PlanC",
            }.OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

            Enumerable.SequenceEqual(nodes, runMap.Nodes.Values.Select(x => x.Name).OrderBy(x => x, StringComparer.OrdinalIgnoreCase)).Should().BeTrue();

            var edges = new[]
            {
                "S1/Start->S1/PlanA",
                "S1/PlanA->S1/PlanB",
                "S2/Start->S2/PlanA",
                "S2/PlanA->S2/PlanC",
            }.OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

            Enumerable.SequenceEqual(edges, runMap.Edges.Values.Select(x => $"{x.FromActivityName}->{x.ToActivityName}").OrderBy(x => x, StringComparer.OrdinalIgnoreCase)).Should().BeTrue();

            await runMap.Run(WorkContextBuilder.Default, new RunContext());

            var sequence = new[]
            {
                "S1/Start",
                "S1/Start-Link-PlanA(1)",
                "S1/PlanA",
                "S1/PlanA-Link-PlanB",
                "S1/PlanB",
                "S2/Start",
                "S2/Start-Link-PlanA(2)",
                "S2/PlanA",
                "S2/PlanA-Link-PlanC",
                "S2/PlanC",
            }
            .GroupBy(x => x.Substring(0, 2))
            .ToList();

            sequence.Count().Should().Be(2);

            var qseq = queue
                .Select(x => x.node)
                .GroupBy(x => x.Substring(0, 2))
                .ToList();

            qseq.Count().Should().Be(2);

            Enumerable.SequenceEqual(sequence.Where(x => x.Key == "S1").SelectMany(x => x), qseq.Where(x => x.Key == "S1").SelectMany(x => x)).Should().BeTrue();
            Enumerable.SequenceEqual(sequence.Where(x => x.Key == "S2").SelectMany(x => x), qseq.Where(x => x.Key == "S2").SelectMany(x => x)).Should().BeTrue();
        }

        [Fact]
        public async Task TestSequenceShortForm_ThreeWithTwoInParallelActivities()
        {
            var queue = new ConcurrentQueue<(string node, int threadId)>();

            var start = new Activity("Start", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));
            var plana = new Activity("PlanA", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));
            var planb = new Activity("PlanB", (c, r) => voidTask(() => queue.Enqueue((r.Activity.Name, Thread.CurrentThread.ManagedThreadId))));

            var builder = new RunMapBuilder()
            {
                new Sequence()
                    + start
                    + (new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Start-PlanA", Thread.CurrentThread.ManagedThreadId)))) + plana)
                    + (new ControlFlow((c, r) => trueTask(() => queue.Enqueue((r.Activity.Name + "-Start-PlanB", Thread.CurrentThread.ManagedThreadId)))) + planb),
            };

            RunMap runMap = builder.Build();

            runMap.Nodes.Count.Should().Be(3);
            runMap.Nodes.Values.Where(x => x.Name == "Start").Count().Should().Be(1);
            runMap.Nodes.Values.Where(x => x.Name == "PlanA").Count().Should().Be(1);
            runMap.Nodes.Values.Where(x => x.Name == "PlanB").Count().Should().Be(1);

            runMap.Edges.Count.Should().Be(2);
            runMap.Edges.Values.Where(x => x.FromActivityName == "Start" && x.ToActivityName == "PlanA").Count().Should().Be(1);
            runMap.Edges.Values.Where(x => x.FromActivityName == "Start" && x.ToActivityName == "PlanB").Count().Should().Be(1);

            await runMap.Run(WorkContextBuilder.Default, new RunContext());

            queue.Count.Should().Be(5);
            queue.Where(x => x.node == "Start").Count().Should().Be(1);
            queue.Where(x => x.node == "PlanA").Count().Should().Be(1);
            queue.Where(x => x.node == "PlanB").Count().Should().Be(1);
            queue.Where(x => x.node == "Start-Start-PlanA").Count().Should().Be(1);
            queue.Where(x => x.node == "Start-Start-PlanB").Count().Should().Be(1);

            var possibleSequence1 = new[]
{
                "Start",
                "PlanA",
                "PlanB",
            };

            var possibleSequence2 = new[]
{
                "Start",
                "PlanB",
                "PlanA",
            };

            var onlyActivities = queue.Where(x => !x.node.Contains("-Start-")).Select(x => x.node);

            (Enumerable.SequenceEqual(possibleSequence1, onlyActivities) || Enumerable.SequenceEqual(possibleSequence2, onlyActivities)).Should().BeTrue();
        }
    }
}
