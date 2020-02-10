// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Toolbox.Test.Orchestrator
{
    public class JobErrorManagment
    {
        private readonly IWorkContext _workContext;
        private readonly ITestOutputHelper _output;

        public JobErrorManagment(ITestOutputHelper output)
        {
            _output = output;

            var telemetryBuilder = new TelemetryServiceBuilder()
                .DoAction(x => _output.WriteLine(x.ToString()))
                .Build();

            _workContext = WorkContextBuilder.Default
                .With(telemetryBuilder.CreateLogger("test"));
        }

        [Fact]
        public void TestExceptionHandling()
        {
            var job1 = new TestJob("Job1") { ThrowException = true };

            var graph = new GraphMap<string, JobBase<string>, IGraphEdge<string>>
            {
                job1,
            };

            var jobHost = new OrchestratorBuilder<string, JobBase<string>, IGraphEdge<string>>(graph)
                .Build()
                .Start(_workContext);

            jobHost.Wait(_workContext);

            jobHost.RunningTask.IsCompleted.Should().BeTrue();
            jobHost.GetProcessedNodeKeys().Count.Should().Be(0);
            jobHost.GetStopNodeKeys().Count.Should().Be(1);

            graph.Nodes.Values
                .All(x => x.GetResult(_workContext).Status == JobStatus.Failed)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void TestException2Handling()
        {
            var job1 = new TestJob("Job1") { ThrowException = true };
            var job1a = new TestJob("Job1-a");
            var job2 = new TestJob("Job2-a", job1);

            var graph = new GraphMap<string, JobBase<string>, IGraphEdge<string>>
            {
                job1,
                job1a,
                job2,
                new GraphEdge<string>(job1.Key, job2.Key)
            };

            var jobHost = new OrchestratorBuilder<string, JobBase<string>, IGraphEdge<string>>(graph)
                .Build()
                .Start(_workContext);

            jobHost.Wait(_workContext);
            jobHost.RunningTask.IsCompleted.Should().BeTrue();
            jobHost.GetProcessedNodeKeys().ForEach(x => _output.WriteLine($"ProcessNode: {x}"));
            jobHost.GetProcessedNodeKeys().Count.Should().Be(1);
            jobHost.GetStopNodeKeys().Count.Should().Be(1);
            jobHost.GetProcessedNodeKeys().Last().Should().Be(job1a.Key);
        }

        private class TestJob : JobBase<string>
        {
            private bool _success = false;
            private List<TestJob> _dependentJobs;
            private Random _random = new Random();

            public TestJob(string key, params TestJob[] dependentJobs)
                : base(key)
            {
                _dependentJobs = dependentJobs.ToList();
            }

            private bool RandomDelay { get; set; }

            public bool ThrowException { get; set; }

            public override IJobResult GetResult(IWorkContext context)
            {
                return new JobResult(_success ? JobStatus.Completed : JobStatus.Failed);
            }

            protected override Task Execute(IWorkContext context, CancellationToken stoppingToken)
            {
                context.Telemetry.Info(context, $"Starting {Key}");

                if (ThrowException)
                {
                    throw new InvalidOperationException("abort");
                }

                // Verify dependent jobs if any
                foreach (var job in _dependentJobs)
                {
                    Verify.Assert(job.GetResult(context).Status == JobStatus.Completed, $"Linked job {job.Key} has not passed");
                }

                if (RandomDelay)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(_random.Next(100, 2000)));
                }

                _success = true;

                return Task.FromResult(0);
            }
        }
    }
}
