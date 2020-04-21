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
    public class SendMessageTests
    {
        private static readonly Func<Action, Task> voidTask = f => { f(); return Task.CompletedTask; };

        [Fact]
        public async Task TestMessageProcessing_ShouldPass()
        {
            int index = 0;

            var start = new Activity("Start", (c, r) => voidTask(() => index = r.GetMessage<int>()));
            var plana = new Activity("PlanA", (c, r) => voidTask(() => index += 10));
            var planb = new Activity("PlanB", (c, r) => voidTask(() => index += 100));

            var builder = new RunMapBuilder()
            {
                new Sequence()
                    + start
                    + (new ControlFlow((c, r) => Task.FromResult(r.GetMessage<int>() % 2 == 0)) + plana)
                    + (new ControlFlow((c, r) => Task.FromResult(r.GetMessage<int>() % 3 == 0)) + planb),
            };

            RunMap runMap = builder.Build();

            await runMap.Send(WorkContextBuilder.Default, 2);
            index.Should().Be(12);

            await runMap.Send(WorkContextBuilder.Default, 3);
            index.Should().Be(103);
        }
    }
}
