using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Dataflow;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Dataflow.Test.Functions
{
    public class InvokeWithMatchTests
    {
        [Fact]
        public async Task GivenFunctions_WhenAllInvokedWithSingleContext_ShouldQueue()
        {
            FunctionHost host = new FunctionHostBuilder()
                .AddFunction(typeof(Receiver1).ToEnumerable().FindMethodsByAttribute<FunctionAttribute>().ToArray())
                .Build();

            IReadOnlyList<IFunction> functions = host.GetFunctions();
            functions.Should().NotBeNull();
            functions.Count.Should().Be(3);

            Receiver1 receiver = functions.First().Instance.CastAs<Receiver1>();

            const string msg = "Message 99-";

            var objs = new object[]
            {
                new SendMsg("private"),
                msg,
            };

            await host["SendFunction1"].InjectAsync(objs);
            await host["SendFunction2"].InjectAsync(objs);
            bool state = await host["SendFunction3"].InjectAsync<bool>(objs);
            state.Should().BeFalse();

            receiver.Queue.Count.Should().Be(3);

            receiver.Queue.TryDequeue(out string? result).Should().BeTrue();
            result.Should().Be(msg);

            receiver.Queue.TryDequeue(out result).Should().BeTrue();
            result.Should().Be(msg + ":private");

            receiver.Queue.TryDequeue(out result).Should().BeTrue();
            result.Should().Be(msg + ":private:False");

            objs = new object[]
            {
                new SendMsg("private"),
                msg,
                true,
            };

            state = await host["SendFunction3"].InjectAsync<bool>(objs);
            state.Should().BeTrue();

            receiver.Queue.Count.Should().Be(1);

            receiver.Queue.TryDequeue(out result).Should().BeTrue();
            result.Should().Be(msg + ":private:True");

            host.Dispose();
            host.GetFunctions().Count.Should().Be(0);
            receiver.Queue.Count.Should().Be(0);
        }

        [Fact]
        public async Task GivenFunction_FigureOutMissingParameter_ShouldCreateAndQueue()
        {
            FunctionHost host = new FunctionHostBuilder()
                .AddFunction(typeof(Receiver1).ToEnumerable().FindMethodsByAttribute<FunctionAttribute>().ToArray())
                .Build();

            IReadOnlyList<IFunction> functions = host.GetFunctions();
            functions.Should().NotBeNull();
            functions.Count.Should().Be(3);

            Receiver1 receiver = functions.First().Instance.CastAs<Receiver1>();

            const string msg = "Message 99-";

            var filteredTypes = new Type[]
            {
                msg.GetType(),
            };

            Type[] missing = host["SendFunction3"].FunctionInfo.MethodInfo.GetMissingParameters(filteredTypes);
            missing.Should().NotBeNull();
            missing.Length.Should().Be(1);
            (missing[0] == typeof(SendMsg)).Should().BeTrue();

            var objs = new object[]
            {
                new SendMsg("private"),
                msg,
            };

            bool state = await host["SendFunction3"].InjectAsync<bool>(objs);
            state.Should().BeFalse();

            receiver.Queue.Count.Should().Be(1);

            receiver.Queue.TryDequeue(out string? result).Should().BeTrue();
            result.Should().Be(msg + ":private:False");

            host.Dispose();
            host.GetFunctions().Count.Should().Be(0);
            receiver.Queue.Count.Should().Be(0);
        }

        public class Receiver1 : IDisposable
        {
            public ConcurrentQueue<string> Queue { get; } = new ConcurrentQueue<string>();

            [Function("SendFunction1")]
            public Task SendFunction1(string message)
            {
                Queue.Enqueue(message);
                return Task.CompletedTask;
            }

            [Function("SendFunction2")]
            public Task SendFunction2(string message, SendMsg sendMsg)
            {
                sendMsg.VerifyNotNull(nameof(sendMsg));

                Queue.Enqueue(message + ":" + sendMsg.Message);
                return Task.CompletedTask;
            }

            [Function("SendFunction3")]
            public Task<bool> Function3(string message, SendMsg sendMsg, bool okay = false)
            {
                sendMsg.VerifyNotNull(nameof(sendMsg));

                Queue.Enqueue(message + ":" + sendMsg.Message + ":" + okay.ToString());
                return Task.FromResult(okay);
            }

            public void Dispose() => Queue.Clear();
        }

        public class SendMsg
        {
            public SendMsg(string message) => Message = message.VerifyNotEmpty(nameof(message));

            public string Message { get; }
        }
    }
}
