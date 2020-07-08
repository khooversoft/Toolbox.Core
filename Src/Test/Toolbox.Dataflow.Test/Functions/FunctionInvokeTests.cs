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
    public class FunctionInvokeTests
    {
        [Fact]
        public void GivenInvalidFunction_WhenHostConstructed_ShouldThrow()
        {
            Action act = () => new FunctionHostBuilder()
                .AddFunction(typeof(InvalidReceiver1).ToEnumerable().FindMethodsByAttribute<FunctionAttribute>().ToArray())
                .Build();

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenInvalidFunctionWithValueType_WhenHostConstructed_ShouldThrow()
        {
            Action act = () => new FunctionHostBuilder()
                .AddFunction(typeof(InvalidReceiver2).ToEnumerable().FindMethodsByAttribute<FunctionAttribute>().ToArray())
                .Build();

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public async Task GivenSingleFunction_WhenSentMessage_ShouldQueue()
        {
            FunctionHost host = new FunctionHostBuilder()
                .AddFunction(typeof(Receiver1).ToEnumerable().FindMethodsByAttribute<FunctionAttribute>().ToArray())
                .Build();

            IReadOnlyList<IFunction> functions = host.GetFunctions();
            functions.Should().NotBeNull();
            functions.Count.Should().Be(1);

            const string msg = "message #1";
            Receiver1 receiver = functions.First().Instance.CastAs<Receiver1>();
            await functions.First().Invoke(msg);

            receiver.Queue.Count.Should().Be(1);
            receiver.Queue.TryDequeue(out string? result).Should().BeTrue();
            result.Should().Be(msg);

            host.Dispose();
            host.GetFunctions().Count.Should().Be(0);
            receiver.Queue.Count.Should().Be(0);
        }

        [Fact]
        public async Task GivenTwoFunction_WhenSentMessages_ShouldQueue()
        {
            FunctionHost host = new FunctionHostBuilder()
                .AddFunction(typeof(Receiver2).ToEnumerable().FindMethodsByAttribute<FunctionAttribute>().ToArray())
                .Build();

            IReadOnlyList<IFunction> functions = host.GetFunctions();
            functions.Should().NotBeNull();
            functions.Count.Should().Be(2);

            Receiver2 receiver = functions.First().Instance.CastAs<Receiver2>();
            const string msg1 = "message #1";
            const string msg2 = "message #2";
            bool state = await functions.First().Invoke<bool>(msg1);
            state.Should().BeTrue();
            await functions.Skip(1).First().Invoke(msg2);

            receiver.Queue.Count.Should().Be(2);
            receiver.Queue.TryDequeue(out string? result1).Should().BeTrue();
            result1.Should().Be(msg1);
            receiver.Queue.TryDequeue(out string? result2).Should().BeTrue();
            result2.Should().Be(msg2);

            host.Dispose();
            host.GetFunctions().Count.Should().Be(0);
            receiver.Queue.Count.Should().Be(0);
        }

        [Fact]
        public void GivenTwoFunction_WhenSentMultipleMessages_ShouldQueue()
        {
            FunctionHost host = new FunctionHostBuilder()
                .AddFunction(typeof(Receiver2).ToEnumerable().FindMethodsByAttribute<FunctionAttribute>().ToArray())
                .Build();

            IReadOnlyList<IFunction> functions = host.GetFunctions();
            functions.Should().NotBeNull();
            functions.Count.Should().Be(2);

            Receiver2 receiver = functions.First().Instance.CastAs<Receiver2>();
            const int max = 100;

            var tasks = Enumerable.Range(0, max)
                .SelectMany(x => host.GetFunctions())
                .Select((x, i) => (msg: $"Message {i}", fun: x))
                .Select((x, i) => (x.msg, task: x.fun.Invoke(x.msg)))
                .ToArray();

            Task.WaitAll(tasks.Select(x => x.task).ToArray());

            receiver.Queue.Count.Should().Be(host.Count * max);

            var source = tasks.Select(x => x.msg).OrderBy(x => x);
            var expected = receiver.Queue.OrderBy(x => x);
            Enumerable.SequenceEqual(source, expected).Should().BeTrue();

            host.Dispose();
            host.GetFunctions().Count.Should().Be(0);
            receiver.Queue.Count.Should().Be(0);
        }

        [Fact]
        public async Task GivenTwoFunctions_WhenCallingWithName_ShouldQueue()
        {
            FunctionHost host = new FunctionHostBuilder()
                .AddFunction(typeof(Receiver3).ToEnumerable().FindMethodsByAttribute<FunctionAttribute>().ToArray())
                .Build();

            IReadOnlyList<IFunction> functions = host.GetFunctions();
            functions.Should().NotBeNull();
            functions.Count.Should().Be(2);

            Receiver3 receiver = functions.First().Instance.CastAs<Receiver3>();

            const string msg1 = "message #1";
            host.TryGetFunction("SendFunction1", out IFunction fun1).Should().BeTrue();
            (await fun1.Invoke<bool>(msg1, true)).Should().BeTrue();

            const string msg2 = "message #2";
            host.TryGetFunction("SendFunction2", out IFunction fun2).Should().BeTrue();
            await fun2.Invoke(msg2);

            receiver.Queue.Count.Should().Be(2);
            receiver.Queue.TryDequeue(out string? result1).Should().BeTrue();
            ("return-" + msg1 == result1).Should().BeTrue();

            receiver.Queue.TryDequeue(out string? result2).Should().BeTrue();
            (msg2 == result2).Should().BeTrue();

            host.Dispose();
            host.GetFunctions().Count.Should().Be(0);
            receiver.Queue.Count.Should().Be(0);
        }

        [Fact]
        public async Task GivenTwoFunctionsNotNamed_WhenCallingWithName_ShouldQueue()
        {
            FunctionHost host = new FunctionHostBuilder()
                .AddFunction(typeof(Receiver4).ToEnumerable().FindMethodsByAttribute<FunctionAttribute>().ToArray())
                .Build();

            IReadOnlyList<IFunction> functions = host.GetFunctions();
            functions.Should().NotBeNull();
            functions.Count.Should().Be(2);

            Receiver4 receiver = functions.First().Instance.CastAs<Receiver4>();

            const string msg1 = "message #1";
            host.TryGetFunction("Receiver4.Function1", out IFunction fun1).Should().BeTrue();
            host["Receiver4.Function1"].Should().Be(fun1);
            (await fun1.Invoke<bool>(msg1, true)).Should().BeTrue();

            const string msg2 = "message #2";
            host.TryGetFunction("Receiver4.Function2", out IFunction fun2).Should().BeTrue();
            host["Receiver4.Function2"].Should().Be(fun2);
            await fun2.Invoke(msg2);

            receiver.Queue.Count.Should().Be(2);
            receiver.Queue.TryDequeue(out string? result).Should().BeTrue();
            ("return-" + msg1 == result).Should().BeTrue();

            receiver.Queue.TryDequeue(out string? result2).Should().BeTrue();
            (msg2 == result2).Should().BeTrue();

            host.Dispose();
            host.GetFunctions().Count.Should().Be(0);
            receiver.Queue.Count.Should().Be(0);
        }

        public class InvalidReceiver1 : IDisposable
        {
            public ConcurrentQueue<string> Queue { get; } = new ConcurrentQueue<string>();

            [Function("SendFunction1")]
            public void SendFunction1(string message)
            {
                Queue.Enqueue(message);
            }

            public void Dispose() => Queue.Clear();
        }

        public class InvalidReceiver2 : IDisposable
        {
            public ConcurrentQueue<string> Queue { get; } = new ConcurrentQueue<string>();

            [Function("SendFunction1")]
            public int SendFunction1(string message)
            {
                Queue.Enqueue(message);
                return 10;
            }

            public void Dispose() => Queue.Clear();
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

            public void Dispose() => Queue.Clear();
        }

        public class Receiver2 : IDisposable
        {
            public ConcurrentQueue<string> Queue { get; } = new ConcurrentQueue<string>();

            [Function("SendFunction1")]
            public Task<bool> SendFunction1(string message)
            {
                Queue.Enqueue(message);
                return Task.FromResult(true);
            }

            [Function("SendFunction2")]
            public Task SendFunction2(string message)
            {
                Queue.Enqueue(message);
                return Task.CompletedTask;
            }

            public void Dispose() => Queue.Clear();
        }

        public class Receiver3 : IDisposable
        {
            public ConcurrentQueue<string> Queue { get; } = new ConcurrentQueue<string>();

            [Function("SendFunction1")]
            public Task<bool> SendFunction1(string message, bool append = false)
            {
                Queue.Enqueue((append ? "return-" : string.Empty) + message);
                return Task.FromResult(true);
            }

            [Function("SendFunction2")]
            public Task SendFunction2(string message)
            {
                Queue.Enqueue(message);
                return Task.CompletedTask;
            }

            public void Dispose() => Queue.Clear();
        }

        public class Receiver4 : IDisposable
        {
            public ConcurrentQueue<string> Queue { get; } = new ConcurrentQueue<string>();

            [Function]
            public Task<bool> Function1(string message, bool append = false)
            {
                Queue.Enqueue((append ? "return-" : string.Empty) + message);
                return Task.FromResult(true);
            }

            [Function]
            public Task Function2(string message)
            {
                Queue.Enqueue(message);
                return Task.CompletedTask;
            }

            public void Dispose() => Queue.Clear();
        }
    }
}