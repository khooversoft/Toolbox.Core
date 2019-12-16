// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Test.Orchestrator
{
    public class SequencePipelineTests
    {
        [Fact]
        public void SimplePipelineTest()
        {
            int i = 0;
            IWorkContext context = WorkContext.Empty;

            Enumerable.Empty<Action<IWorkContext>>()
                .Append(x => i += 2)
                .ForEach(x => x.Invoke(context));

            i.Should().Be(2);
        }

        [Fact]
        public async Task AsyncSimplePipelineTest()
        {
            IWorkContext context = WorkContext.Empty;
            var execContext = new ExecutionContext();

            await Enumerable.Empty<Func<IWorkContext, Task>>()
                .Append(x => execContext.AddOne(context))
                .Append(x => execContext.AddTwo(context))
                .ForEachAsync(x => x.Invoke(context));

            execContext.Value.Should().Be(3);
        }

        [Fact]
        public async Task AsyncSimplePipeline2Test()
        {
            IWorkContext context = WorkContext.Empty;
            var execContext = new ExecutionContext(5);

            await Enumerable.Empty<Func<IWorkContext, Task>>()
                .Append(async x => await execContext.AddOne(context))
                .Append(async x => await execContext.AddTwo(context))
                .Append(async x => await execContext.AddOne(context))
                .ForEachAsync(async x => await x.Invoke(context));

            execContext.Value.Should().Be(9);
        }

        private Task<int> Return5()
        {
            return Task.FromResult(5);
        }

        private class ExecutionContext
        {
            public ExecutionContext()
            {
            }

            public ExecutionContext(int value)
            {
                Value = value;
            }

            public int Value { get; private set; }

            public Task AddOne(IWorkContext context)
            {
                Value++;
                return Task.FromResult(0);
            }

            public Task AddTwo(IWorkContext context)
            {
                Value += 2;
                return Task.FromResult(0);
            }
        }
    }
}
