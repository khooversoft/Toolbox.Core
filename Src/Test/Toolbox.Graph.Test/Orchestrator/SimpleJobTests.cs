// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox;
using Xunit;
using Xunit.Abstractions;

namespace Toolbox.Test.Orchestrator
{
    public class SimpleJobTests
    {
        private readonly IWorkContext _workContext;
        private readonly ITestOutputHelper _output;

        public SimpleJobTests(ITestOutputHelper output)
        {
            _output = output;

            var telemetryBuilder = new TelemetryServiceBuilder()
                .DoAction(x => _output.WriteLine(x.ToString()))
                .Build();

            _workContext = WorkContextBuilder.Default
                .With(telemetryBuilder.CreateLogger("test"));
        }

        [Fact]
        public void SetupJobTest()
        {
            var graph = new GraphMap<string, JobBase<string>, IGraphEdge<string>>
            {
                new TestJob("Job1")
            };

            var jobHost = new OrchestratorBuilder<string, JobBase<string>, IGraphEdge<string>>(graph)
                .Build()
                .Start(_workContext);

            jobHost.Wait(_workContext);
            jobHost.RunningTask.IsCompleted.Should().BeTrue();
            jobHost.GetProcessedNodeKeys().ForEach(x => _output.WriteLine($"ProcessNode: {x}"));
            jobHost.GetProcessedNodeKeys().Count.Should().Be(1);

            graph.Nodes.Values
                .All(x => x.GetResult(_workContext).Status == JobStatus.Completed)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void TwoSerialJobTest()
        {
            var job1 = new TestJob("Job1");
            var job2 = new TestJob("Job2");

            var graph = new GraphMap<string, JobBase<string>, IGraphEdge<string>>
            {
                job1,
                job2,
                new GraphEdge<string>(job1.Key, job2.Key)
            };

            var jobHost = new OrchestratorBuilder<string, JobBase<string>, IGraphEdge<string>>(graph)
                .Build()
                .Start(_workContext);

            jobHost.Wait(_workContext);
            jobHost.RunningTask.IsCompleted.Should().BeTrue();
            jobHost.GetProcessedNodeKeys().ForEach(x => _output.WriteLine($"ProcessNode: {x}"));
            jobHost.GetProcessedNodeKeys().Count.Should().Be(2);
            jobHost.GetProcessedNodeKeys()[0].Should().Be(job1.Key);
            jobHost.GetProcessedNodeKeys()[1].Should().Be(job2.Key);

            graph.Nodes.Values
                .All(x => x.GetResult(_workContext).Status == JobStatus.Completed)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ThreeMix1JobTest()
        {
            var job1 = new TestJob("Job1" );
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
            jobHost.GetProcessedNodeKeys().Count.Should().Be(3);
            jobHost.GetProcessedNodeKeys().Last().Should().Be(job2.Key);

            graph.Nodes.Values
                .All(x => x.GetResult(_workContext).Status == JobStatus.Completed)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ThreeMix2JobTest()
        {
            var job1 = new TestJob("Job1");
            var job1a = new TestJob("Job1-a");
            var job2 = new TestJob("Job2", job1);
            var job2a = new TestJob("Job2-a", job2);

            var graph = new GraphMap<string, JobBase<string>, IGraphEdge<string>>
            {
                job1,
                job1a,
                job2,
                job2a,
                new GraphEdge<string>(job1.Key, job2.Key),
                new GraphEdge<string>(job2.Key, job2a.Key),
            };

            var jobHost = new OrchestratorBuilder<string, JobBase<string>, IGraphEdge<string>>(graph)
                .Build()
                .Start(_workContext);

            jobHost.Wait(_workContext);
            jobHost.RunningTask.IsCompleted.Should().BeTrue();
            jobHost.GetProcessedNodeKeys().ForEach(x => _output.WriteLine($"ProcessNode: {x}"));
            jobHost.GetProcessedNodeKeys().Count.Should().Be(4);
            jobHost.GetProcessedNodeKeys().Last().Should().Be(job2a.Key);

            graph.Nodes.Values
                .All(x => x.GetResult(_workContext).Status == JobStatus.Completed)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ThreeMix3JobTest()
        {
            var job1 = new TestJob("Job1");
            var job1a = new TestJob("Job1-a");
            var job2 = new TestJob("Job2", job1);
            var job2a = new TestJob("Job2-a", job1);

            var graph = new GraphMap<string, JobBase<string>, IGraphEdge<string>>
            {
                job1,
                job1a,
                job2,
                job2a,
                new GraphEdge<string>(job1.Key, job2.Key),
                new GraphEdge<string>(job1.Key, job2a.Key),
            };

            var jobHost = new OrchestratorBuilder<string, JobBase<string>, IGraphEdge<string>>(graph)
                .Build()
                .Start(_workContext);

            jobHost.Wait(_workContext);
            jobHost.RunningTask.IsCompleted.Should().BeTrue();
            jobHost.GetProcessedNodeKeys().ForEach(x => _output.WriteLine($"ProcessNode: {x}"));
            jobHost.GetProcessedNodeKeys().Count.Should().Be(4);

            graph.Nodes.Values
                .All(x => x.GetResult(_workContext).Status == JobStatus.Completed)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ThreeMix3WithDelayJobTest()
        {
            var job1 = new TestJob("Job1") { RandomDelay = true };
            var job1a = new TestJob("Job1-a") { RandomDelay = true }; ;
            var job2 = new TestJob("Job2", job1) { RandomDelay = true }; ;
            var job2a = new TestJob("Job2-a", job1) { RandomDelay = true }; ;

            var graph = new GraphMap<string, JobBase<string>, IGraphEdge<string>>
            {
                job1,
                job1a,
                job2,
                job2a,
                new GraphEdge<string>(job1.Key, job2.Key),
                new GraphEdge<string>(job1.Key, job2a.Key),
            };

            var jobHost = new OrchestratorBuilder<string, JobBase<string>, IGraphEdge<string>>(graph)
                .Build()
                .Start(_workContext);

            jobHost.Wait(_workContext);
            jobHost.RunningTask.IsCompleted.Should().BeTrue();
            jobHost.GetProcessedNodeKeys().ForEach(x => _output.WriteLine($"ProcessNode: {x}"));
            jobHost.GetProcessedNodeKeys().Count.Should().Be(4);

            graph.Nodes.Values
                .All(x => x.GetResult(_workContext).Status == JobStatus.Completed)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Three2LevelsDeepTest()
        {
            var job1 = new TestJob("Job1");
            var job1a = new TestJob("Job1-a");
            var job2 = new TestJob("Job2", job1);
            var job2a = new TestJob("Job2-a", job1);
            var job3 = new TestJob("Job3", job2);

            var graph = new GraphMap<string, JobBase<string>, IGraphEdge<string>>
            {
                job1,
                job1a,
                job2,
                job2a,
                job3,
                new GraphEdge<string>(job1.Key, job2.Key),
                new GraphEdge<string>(job1.Key, job2a.Key),
                new GraphEdge<string>(job2.Key, job3.Key),
            };

            var jobHost = new OrchestratorBuilder<string, JobBase<string>, IGraphEdge<string>>(graph)
                .Build()
                .Start(_workContext);

            jobHost.Wait(_workContext);
            jobHost.RunningTask.IsCompleted.Should().BeTrue();
            jobHost.GetProcessedNodeKeys().ForEach(x => _output.WriteLine($"ProcessNode: {x}"));
            jobHost.GetProcessedNodeKeys().Count.Should().Be(5);

            graph.Nodes.Values
                .All(x => x.GetResult(_workContext).Status == JobStatus.Completed)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void DeepDependencyTest()
        {
            var job1 = new TestJob("Job1");
            var job1a = new TestJob("Job1-a");
            var job2 = new TestJob("Job2", job1);
            var job2a = new TestJob("Job2-a", job1);
            var job3 = new TestJob("Job3", job2);
            var lastJob = new TestJob("Last", job1, job3);

            var graph = new GraphMap<string, JobBase<string>, IGraphEdge<string>>
            {
                job1,
                job1a,
                job2,
                job2a,
                job3,
                lastJob,
                new GraphEdge<string>(job1.Key, job2.Key),
                new GraphEdge<string>(job1.Key, job2a.Key),
                new GraphEdge<string>(job2.Key, job3.Key),
                new GraphEdge<string>(job1.Key, lastJob.Key),
                new GraphEdge<string>(job3.Key, lastJob.Key),
            };

            var jobHost = new OrchestratorBuilder<string, JobBase<string>, IGraphEdge<string>>(graph)
                .Build()
                .Start(_workContext);

            jobHost.Wait(_workContext);
            jobHost.RunningTask.IsCompleted.Should().BeTrue();
            jobHost.GetProcessedNodeKeys().ForEach(x => _output.WriteLine($"ProcessNode: {x}"));
            jobHost.GetProcessedNodeKeys().Count.Should().Be(6);

            graph.Nodes.Values
                .All(x => x.GetResult(_workContext).Status == JobStatus.Completed)
                .Should()
                .BeTrue();
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

            public bool RandomDelay { get; set; }

            public override IJobResult GetResult(IWorkContext context)
            {
                return new JobResult(_success ? JobStatus.Completed : JobStatus.Failed);
            }

            protected override Task Execute(IWorkContext context, CancellationToken stoppingToken)
            {
                context.Telemetry.Info(context, $"Starting {Key}");

                // Verify dependent jobs if any
                foreach(var job in _dependentJobs)
                {
                    Verify.Assert(job.GetResult(context).Status == JobStatus.Completed, $"Linked job {job.Key} has not passed");
                }

                if( RandomDelay)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(_random.Next(100, 2000)));
                }

                _success = true;

                return Task.FromResult(0);
            }
        }
    }
}
