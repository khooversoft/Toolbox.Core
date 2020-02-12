using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class PipelineTests
    {
        private readonly static IWorkContext _workContext = WorkContextBuilder.Default;

        [Fact]
        public async Task GivenSimplePipeline_WithoutExit_ShouldThrow()
        {
            int counter = 0;

            var pipeline = new PipelineBuilder<int>()
                .Use((context, message) =>
                {
                    counter += message;
                    return Task.CompletedTask;
                })
                .Build();

            Func<Task> act = async () => await pipeline(_workContext, 10);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GivenSimplePipeline_WhenMessageSent_ShouldReceive()
        {
            int counter = 0;

            var pipeline = new PipelineBuilder<int>()
                .Use((context, message, next) =>
                {
                    counter += message;
                    return Task.CompletedTask;
                })
                .Build();

            await pipeline(_workContext, 10);

            counter.Should().Be(10);
        }

        [Fact]
        public async Task GivenSimpleMappedPipeline_WhenMessageSent_ShouldReceive()
        {
            int evenCounter = 0;
            int oddCounter = 0;

            var pipeline = new PipelineBuilder<int>()
                .Map(x => x % 2 == 0, (context, message) =>
                {
                    evenCounter += message;
                    return Task.CompletedTask;
                })
                .Map(x => x % 2 != 0, (context, message) =>
                {
                    oddCounter += message;
                    return Task.CompletedTask;
                })
                .Build();

            await pipeline(_workContext, 10);

            evenCounter.Should().Be(10);
            oddCounter.Should().Be(0);

            await pipeline(_workContext, 9);

            evenCounter.Should().Be(10);
            oddCounter.Should().Be(9);
        }

        [Fact]
        public async Task GivenSimpleMappedPipeline_WhenMultipleMessagesSent_ShouldReceive()
        {
            int evenCounter = 0;
            int oddCounter = 0;
            const int max = 10;

            var pipeline = new PipelineBuilder<int>()
                .Map(x => x % 2 == 0, (context, message, next) =>
                {
                    evenCounter += message;
                    return Task.CompletedTask;
                })
                .Map(x => x % 2 != 0, (context, message, next) =>
                {
                    oddCounter += message;
                    return Task.CompletedTask;
                })
                .Build();

            await Enumerable.Range(0, max)
                .ForEachAsync(async x => await pipeline(_workContext, x));

            evenCounter.Should().Be(20);
            oddCounter.Should().Be(25);
        }

        [Fact]
        public void GivenSimpleMappedPipeline_WhenMultipleMessagesSentAsync_ShouldReceive()
        {
            int evenCounter = 0;
            int oddCounter = 0;
            const int max = 10;

            var pipeline = new PipelineBuilder<int>()
                .Map(x => x % 2 == 0, (context, message, next) =>
                {
                    Interlocked.Add(ref evenCounter, message);
                    return Task.CompletedTask;
                })
                .Map(x => x % 2 != 0, (context, message, next) =>
                {
                    Interlocked.Add(ref oddCounter, message);
                    return Task.CompletedTask;
                })
                .Build();

            var list = Enumerable.Range(0, max)
                .Select(x => Task.Run(() => pipeline(_workContext, x)))
                .ToArray();

            Task.WaitAll(list);

            evenCounter.Should().Be(20);
            oddCounter.Should().Be(25);
        }

        [Fact]
        public async Task GivenSimplePipeline_WhenMessageIsTransformed_ShouldReceive()
        {
            int counter = 0;

            var pipeline = new PipelineBuilder<int>()
                .Use((context, message, next) =>
                {
                    return next(context, message + 10);
                })
                .Use((context, message, next) =>
                {
                    counter += message;
                    return Task.CompletedTask;
                })
                .Build();

            await pipeline(_workContext, 10);

            counter.Should().Be(20);
        }
    }
}
