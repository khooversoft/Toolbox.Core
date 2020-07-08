// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Dataflow;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Dataflow.Test.Pipelines
{
    public class PipelineTests
    {
        [Fact]
        public async Task GivenSimplePipeline_WithoutExit_ShouldThrow()
        {
            int counter = 0;

            Func<int, Task> pipeline = new PipelineBuilder<int>()
                .Use((message) =>
                {
                    counter += message;
                    return Task.CompletedTask;
                })
                .Build();

            Func<Task> act = async () => await pipeline(10);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GivenSimplePipeline_WhenMessageSent_ShouldReceive()
        {
            int counter = 0;

            Func<int, Task> pipeline = new PipelineBuilder<int>()
                .Use((message, next) =>
                {
                    counter += message;
                    return Task.CompletedTask;
                })
                .Build();

            await pipeline(10);

            counter.Should().Be(10);
        }

        [Fact]
        public async Task GivenSimpleMappedPipeline_WhenMessageSent_ShouldReceive()
        {
            int evenCounter = 0;
            int oddCounter = 0;

            Func<int, Task> pipeline = new PipelineBuilder<int>()
                .Map(x => x % 2 == 0, (message) =>
                {
                    evenCounter += message;
                    return Task.CompletedTask;
                })
                .Map(x => x % 2 != 0, (message) =>
                {
                    oddCounter += message;
                    return Task.CompletedTask;
                })
                .Build();

            await pipeline(10);

            evenCounter.Should().Be(10);
            oddCounter.Should().Be(0);

            await pipeline(9);

            evenCounter.Should().Be(10);
            oddCounter.Should().Be(9);
        }

        [Fact]
        public async Task GivenSimpleMappedPipeline_WhenMultipleMessagesSent_ShouldReceive()
        {
            int evenCounter = 0;
            int oddCounter = 0;
            const int max = 10;

            Func<int, Task> pipeline = new PipelineBuilder<int>()
                .Map(x => x % 2 == 0, (message, next) =>
                {
                    evenCounter += message;
                    return Task.CompletedTask;
                })
                .Map(x => x % 2 != 0, (message, next) =>
                {
                    oddCounter += message;
                    return Task.CompletedTask;
                })
                .Build();

            await Enumerable.Range(0, max)
                .ForEachAsync(async x => await pipeline(x));

            evenCounter.Should().Be(20);
            oddCounter.Should().Be(25);
        }

        [Fact]
        public void GivenSimpleMappedPipeline_WhenMultipleMessagesSentAsync_ShouldReceive()
        {
            int evenCounter = 0;
            int oddCounter = 0;
            const int max = 10;

            Func<int, Task> pipeline = new PipelineBuilder<int>()
                .Map(x => x % 2 == 0, (message, next) =>
                {
                    Interlocked.Add(ref evenCounter, message);
                    return Task.CompletedTask;
                })
                .Map(x => x % 2 != 0, (message, next) =>
                {
                    Interlocked.Add(ref oddCounter, message);
                    return Task.CompletedTask;
                })
                .Build();

            var list = Enumerable.Range(0, max)
                .Select(x => Task.Run(() => pipeline(x)))
                .ToArray();

            Task.WaitAll(list);

            evenCounter.Should().Be(20);
            oddCounter.Should().Be(25);
        }

        [Fact]
        public async Task GivenSimplePipeline_WhenMessageIsTransformed_ShouldReceive()
        {
            int counter = 0;

            Func<int, Task> pipeline = new PipelineBuilder<int>()
                .Use((message, next) =>
                {
                    return next(message + 10);
                })
                .Use((message, next) =>
                {
                    counter += message;
                    return Task.CompletedTask;
                })
                .Build();

            await pipeline(10);

            counter.Should().Be(20);
        }
    }
}
