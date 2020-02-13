// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

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
    public class DataflowWithPredicateTests
    {
        [Fact]
        public async Task GivenSingleRoute_WhenMessageSent_ShouldReceive()
        {
            int count = 0;
            const int max = 10;

            IDataflowSource<int> dataflow = new DataflowBuilder<int>()
            {
                new ActionDataflow<int>(x => count++, x => x % 2 == 0)
            }.Build();

            await Enumerable.Range(0, max)
                .ForEachAsync(async x => await dataflow.PostAsync(x));

            dataflow.Complete();
            dataflow.Completion.Wait();

            count.Should().Be(max / 2);
        }

        [Fact]
        public async Task GivenTwoRoute_WhenMessageSent_ShouldReceive()
        {
            int evenCount = 0;
            int oddCount = 0;
            const int max = 10;

            IDataflowSource<int> dataflow = new DataflowBuilder<int>()
            {
                new ActionDataflow<int>(x => Interlocked.Increment(ref evenCount), x => x % 2 == 0),
                new ActionDataflow<int>(x => Interlocked.Increment(ref oddCount), x => x % 2 != 0),
            }.Build();

            await Enumerable.Range(0, max)
                .ForEachAsync(async x => await dataflow.PostAsync(x));

            dataflow.Complete();
            dataflow.Completion.Wait();

            evenCount.Should().Be(max / 2);
            oddCount.Should().Be(max / 2);
        }

        [Fact]
        public async Task GivenTwoRoute_WhenMessageAwait_ShouldReceive()
        {
            int evenCount = 0;
            int oddCount = 0;
            const int max = 1000;

            IDataflowSource<int> dataflow = new DataflowBuilder<int>()
            {
                new ActionDataflow<int>(x => Interlocked.Increment(ref evenCount), x => x % 2 == 0),
                new ActionDataflow<int>(x => Interlocked.Increment(ref oddCount), x => x % 2 != 0),
            }.Build();

            for (int i = 0; i < max; i++)
            {
                await dataflow.PostAsync(i);
            }

            dataflow.Complete();
            dataflow.Completion.Wait();

            evenCount.Should().Be(max / 2);
            oddCount.Should().Be(max / 2);
        }

        [Fact]
        public void GivenTwoRoute_WhenMessageSentOnDifferentTask_ShouldReceive()
        {
            int evenCount = 0;
            int oddCount = 0;
            const int max = 1000;

            IDataflowSource<int> dataflow = new DataflowBuilder<int>()
            {
                new ActionDataflow<int>(x => Interlocked.Increment(ref evenCount), x => x % 2 == 0),
                new ActionDataflow<int>(x => Interlocked.Increment(ref oddCount), x => x % 2 != 0),
            }.Build();

            var tasks = Enumerable.Range(0, max)
                .Select(x => Task.Run(() => dataflow.PostAsync(x)))
                .ToArray();

            Task.WaitAll(tasks);
            
            dataflow.Complete();
            dataflow.Completion.Wait();

            evenCount.Should().Be(max / 2);
            oddCount.Should().Be(max / 2);
        }
    }
}
