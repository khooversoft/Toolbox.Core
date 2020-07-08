using FluentAssertions;
using Khooversoft.Toolbox.Run;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Run.Test.Patterns
{
    public class OrPatternTests
    {
        private static readonly Func<Action, Task> voidTask = f => { f(); return Task.CompletedTask; };
        private static readonly Func<Action, Task<bool>> trueTask = f => { f(); return Task.FromResult(true); };
        private static readonly Func<Action, Task<bool>> falseTask = f => { f(); return Task.FromResult(false); };

        [Fact]
        public async Task TestSimpleOrSequence_ShouldPass()
        {
            var queue = new ConcurrentQueue<(string node, int threadId)>();

            var start = new Activity("Start", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var plana = new Activity("PlanA", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var planb = new Activity("PlanB", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));

            var builder = new RunMapBuilder()
            {
                new Sequence()
                    + start
                    + (new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Start-PlanA", Thread.CurrentThread.ManagedThreadId)))) + plana)
                    + (new ControlFlow((r, a) => falseTask(() => queue.Enqueue((a.Name + "-Start-PlanB", Thread.CurrentThread.ManagedThreadId)))) + planb),
            };

            RunMap runMap = builder.Build();

            await runMap.Run();

            var possibleSequence = new[]
{
                "Start",
                "PlanA",
            };

            var onlyActivities = queue.Where(x => !x.node.Contains("-Start-")).Select(x => x.node);

            Enumerable.SequenceEqual(possibleSequence, onlyActivities).Should().BeTrue();
        }

        [Fact]
        public async Task TestSimpleOrSequence2_ShouldPass()
        {
            var queue = new ConcurrentQueue<(string node, int threadId)>();

            var start = new Activity("Start", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var plana = new Activity("PlanA", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var planb = new Activity("PlanB", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));

            var builder = new RunMapBuilder()
            {
                new Sequence()
                    + start
                    + (new ControlFlow((r, a) => falseTask(() => queue.Enqueue((a.Name + "-Start-PlanA", Thread.CurrentThread.ManagedThreadId)))) + plana)
                    + (new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Start-PlanB", Thread.CurrentThread.ManagedThreadId)))) + planb),
            };

            RunMap runMap = builder.Build();

            await runMap.Run();

            var possibleSequence = new[]
{
                "Start",
                "PlanB",
            };

            var onlyActivities = queue.Where(x => !x.node.Contains("-Start-")).Select(x => x.node);

            Enumerable.SequenceEqual(possibleSequence, onlyActivities).Should().BeTrue();
        }

        [Fact]
        public async Task TestErrorSequence_ShouldPass()
        {
            var queue = new ConcurrentQueue<(string node, int threadId)>();

            var start = new Activity("Start", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var plana = new Activity("PlanA", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var planb = new Activity("PlanB", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var planc = new Activity("PlanC", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var error = new Activity("Error", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var restart = new Activity("Restart", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));

            var builder = new RunMapBuilder()
            {
                new Sequence()
                    + start
                    + new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Link-PlanA", Thread.CurrentThread.ManagedThreadId))))
                    + (new ControlFlow((r, a) => falseTask(() => queue.Enqueue((a.Name + "-Link-Error(1)", Thread.CurrentThread.ManagedThreadId)))) + error)
                    + plana
                    + new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Link-PlanB", Thread.CurrentThread.ManagedThreadId))))
                    + (new ControlFlow((r, a) => falseTask(() => queue.Enqueue((a.Name + "-Link-Error(2)", Thread.CurrentThread.ManagedThreadId)))) + error)
                    + planb
                    + new ControlFlow((r, a) => falseTask(() => queue.Enqueue((a.Name + "-Link-PlanB", Thread.CurrentThread.ManagedThreadId))))
                    + (new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Link-Error(3)", Thread.CurrentThread.ManagedThreadId)))) + error)
                    + planc,

                new Sequence()
                    + error
                    + new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Link-Restart", Thread.CurrentThread.ManagedThreadId))))
                    + restart
            };

            RunMap runMap = builder.Build();

            var expectedNodes = new string[]
            {
                "Start",
                "PlanA",
                "PlanB",
                "PlanC",
                "Error",
                "Restart",
            }.OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

            var nodes = runMap.Nodes.Values.Select(x => x.Name).OrderBy(x => x, StringComparer.OrdinalIgnoreCase);
            Enumerable.SequenceEqual(expectedNodes, nodes).Should().BeTrue();

            var expectedEdges = new (string from, string to)[]
            {
                ("Start", "PlanA"),
                ("PlanA", "PlanB"),
                ("PlanB", "PlanC"),
                ("Start", "Error"),
                ("PlanA", "Error"),
                ("PlanB", "Error"),
                ("Error", "Restart"),
            }.OrderBy(x => x.from, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.to, StringComparer.OrdinalIgnoreCase);

            var edges = runMap.Edges.Values.Select(x => (from: x.FromActivityName, to: x.ToActivityName))
                .OrderBy(x => x.from, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.to, StringComparer.OrdinalIgnoreCase);

            Enumerable.SequenceEqual(expectedEdges, edges).Should().BeTrue();

            await runMap.Run();

            var possibleSequence = new[]
{
                "Start",
                "PlanA",
                "PlanB",
                "Error",
                "Restart",
            };

            var onlyActivities = queue.Where(x => !x.node.Contains("-Link-")).Select(x => x.node);

            Enumerable.SequenceEqual(possibleSequence, onlyActivities).Should().BeTrue();
        }

        [Fact]
        public async Task TestRestartSequence_ShouldPass()
        {
            var queue = new ConcurrentQueue<(string node, int threadId)>();

            var start = new Activity("Start", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var plana = new Activity("PlanA", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var planb = new Activity("PlanB", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var planc = new Activity("PlanC", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var error = new Activity("Error", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));
            var restart = new Activity("Restart", (r, a) => voidTask(() => queue.Enqueue((a.Name, Thread.CurrentThread.ManagedThreadId))));

            var builder = new RunMapBuilder()
            {
                new Sequence()
                    + start
                    + new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Link-PlanA", Thread.CurrentThread.ManagedThreadId))))
                    + (new ControlFlow((r, a) => falseTask(() => queue.Enqueue((a.Name + "-Link-Error(1)", Thread.CurrentThread.ManagedThreadId)))) + error)
                    + plana
                    + new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Link-PlanB", Thread.CurrentThread.ManagedThreadId))))
                    + (new ControlFlow((r, a) => falseTask(() => queue.Enqueue((a.Name + "-Link-Error(2)", Thread.CurrentThread.ManagedThreadId)))) + error)
                    + planb
                    + new ControlFlow((r, a) => falseTask(() => queue.Enqueue((a.Name + "-Link-PlanB", Thread.CurrentThread.ManagedThreadId))))
                    + (new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Link-Error(3)", Thread.CurrentThread.ManagedThreadId)))) + error)
                    + planc,

                new Sequence()
                    + error
                    + new ControlFlow(async (r, a) =>
                    {
                        CountContext context = r.Property.Get<CountContext>();
                        if( !context.CanRestart()) return false;

                        queue.Enqueue((a.Name + "-Link-Restart", Thread.CurrentThread.ManagedThreadId));
                        await Task.Delay(TimeSpan.FromMilliseconds(200));
                        return true;
                    })
                    + restart
                    + new ControlFlow((r, a) => trueTask(() => queue.Enqueue((a.Name + "-Link-Start", Thread.CurrentThread.ManagedThreadId))))
                    + start,
            };

            RunMap runMap = builder.Build();

            IRunContext runContext = new RunContext();
            runContext.Property.Set(new CountContext());

            await runMap.Run(runContext, "Start");

            var possibleSequence = new[]
{
                "Start",
                "PlanA",
                "PlanB",
                "Error",
                "Restart",
                "Start",
                "PlanA",
                "PlanB",
                "Error",
            };

            var onlyActivities = queue.Where(x => !x.node.Contains("-Link-")).Select(x => x.node);

            Enumerable.SequenceEqual(possibleSequence, onlyActivities).Should().BeTrue();
        }

        private class CountContext
        {
            private int _count = 0;

            public bool CanRestart() => Interlocked.Increment(ref _count) < 2;
        }
    }
}
