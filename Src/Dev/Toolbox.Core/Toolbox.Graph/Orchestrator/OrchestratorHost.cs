// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Graph
{
    public class OrchestratorHost<TKey, TNode, TEdge>
        where TNode : IGraphNode<TKey>, IJob
        where TEdge : IGraphEdge<TKey>
    {
        private readonly GraphMap<TKey, TNode, TEdge> _graph;
        private readonly IJobHost _jobHost;
        private int _running = 0;
        private readonly Dictionary<Guid, TKey> _processedDict = new Dictionary<Guid, TKey>();
        private GraphTopologicalContext<TKey, TEdge> _graphContext;
        private readonly HashSet<TKey> _runningKeys;
        private readonly List<IJobResult> _jobResults = new List<IJobResult>();
        private readonly int? _maxParallel;
        private readonly SemaphoreSlim _semaphore;

        private object _lock = new object();

        /// <summary>
        /// Create orchestrator host
        /// </summary>
        /// <param name="graph">graph to be executed</param>
        /// <param name="jobHost">job host to use</param>
        /// <param name="maxParallel">max number of parallel task, if null or 0, there is no limit</param>
        public OrchestratorHost(GraphMap<TKey, TNode, TEdge> graph, IJobHost jobHost, int? maxParallel = null)
        {
            graph.Verify(nameof(graph)).IsNotNull();
            jobHost.Verify(nameof(jobHost)).IsNotNull();
            maxParallel.Verify(nameof(maxParallel)).Assert(x => x == null || x >= 0, "value must be greater then or equal to zero");

            _graph = graph;
            _jobHost = jobHost;

            _runningKeys = new HashSet<TKey>(_graph.KeyCompare);
            _maxParallel = maxParallel;
            _semaphore = maxParallel > 0 ? _semaphore = new SemaphoreSlim((int)maxParallel, (int)maxParallel) : null;
        }

        public bool IsRunning => _running == 1;

        public Task RunningTask { get; private set; }

        public IReadOnlyCollection<TKey> RunningKeys => _runningKeys;

        public IReadOnlyList<TKey> GetProcessedNodeKeys() => _graphContext.ProcessedNodeKeys.ToList();

        public IReadOnlyList<TKey> GetStopNodeKeys() => _graphContext.StopNodeKeys.ToList();

        public IReadOnlyList<IJobResult> GetJobResults() => _jobResults.ToList();

        /// <summary>
        /// Start orchestrator host, jobs will be started based on their directed graph edges
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>this</returns>
        public OrchestratorHost<TKey, TNode, TEdge> Start(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();

            int running = Interlocked.CompareExchange(ref _running, 1, 0);
            if (running == 1)
            {
                return this;
            }

            _processedDict.Clear();
            _runningKeys.Clear();
            _graphContext = new GraphTopologicalContext<TKey, TEdge>(maxLevels: 1, equalityComparer: _graph.KeyCompare);

            context.Telemetry.Verbose(context, "Starting Orchestrator Host");

            RunningTask = Task.Run(() => RunJobGraph(context))
                .ContinueWith(x => _running = 0);

            context.Telemetry.Verbose(context, "Started Orchestrator Host");
            return this;
        }

        /// <summary>
        /// Return a list of running nodes in the job host
        /// </summary>
        /// <returns>list of nodes</returns>
        public IReadOnlyList<TNode> GetRunningNodes()
        {
            return _runningKeys
                .Join(_graph.Nodes.Values, x => x, x => x.Key, (o, i) => i)
                .ToList();
        }

        /// <summary>
        /// Wait for graph to complete and all tasks associated
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="timeout">timeout or null for infinite</param>
        /// <returns>true if all tasks are done, false if time out expired</returns>
        public bool Wait(IWorkContext context, TimeSpan? timeout = null)
        {
            if (_running == 0)
            {
                context.Telemetry.Verbose(context, "OrchestratorHost: (Wait) not running, return false");
                return false;
            }

            if (_running != 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }

            timeout = timeout ?? TimeSpan.FromMilliseconds(-1);
            context.Telemetry.Verbose(context, $"OrchestratorHost: (Wait) timeout={timeout}");
            bool status = Task.WaitAll(new Task[] { RunningTask }, (int)((TimeSpan)timeout).TotalMilliseconds, context.CancellationToken);
            context.Telemetry.Verbose(context, $"OrchestratorHost: (Wait) return status {status}");

            return status;
        }

        private async Task RunJobGraph(IWorkContext context)
        {
            context.Telemetry.Verbose(context, $"Enter-{nameof(RunJobGraph)}");
            IWorkContext nodeContext = context.WithNewCv();

            while (true)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var nodesToRun = _graph.TopologicalSort(_graphContext);
                if (nodesToRun.Count == 0)
                {
                    context.Telemetry.Verbose(context, $"Exit-{nameof(RunJobGraph)}, no nodes to run");
                    break;
                }

                IList<TNode> nodesToStart;
                lock (_lock)
                {
                    nodesToStart = nodesToRun[0]
                        .Where(x => !_runningKeys.Contains(x.Key))
                        .ToList();
                }

                foreach (var node in nodesToStart)
                {
                    if (!node.Active)
                    {
                        context.Telemetry.Verbose(context, $"Skipping node {node.Key} because it not active");

                        lock (_lock)
                        {
                            _graphContext.AddProcessedNodeKey(node.Key);
                        }

                        continue;
                    }

                    Guid jobId;
                    if (_semaphore != null)
                    {
                        await _semaphore.WaitAsync();
                        context.Telemetry.TrackMetric(context, $"{nameof(RunJobGraph)}:Enter-Semaphore-CurrentCount", (double)_semaphore.CurrentCount);
                    }

                    nodeContext = nodeContext
                        .WithCreateLogger(node.Key.ToString())
                        .WithIncrement();

                    context.Telemetry.Verbose(context, $"{nameof(RunJobGraph)} Starting job for node {node.Key}, node's cv={nodeContext.Cv}");

                    jobId = await _jobHost.StartJob(nodeContext, node, UpdateCompletionList);

                    context.Telemetry.ActivityStart(context, $"{nameof(RunJobGraph)} Started JobId={jobId.ToString()} for node {node.Key}");

                    lock (_lock)
                    {
                        _runningKeys.Add(node.Key);
                        _processedDict.Add(jobId, node.Key);
                    }
                }

                _jobHost.WaitAny(context);
                context.Telemetry.Verbose(context, $"Task-Completed-{nameof(RunJobGraph)}");
            }

            _jobHost.WaitAll(context);
            context.Telemetry.Verbose(context, $"Completed-{nameof(RunJobGraph)}");
        }

        private void UpdateCompletionList(IWorkContext context, IJobResult jobResult)
        {
            context.Verify(nameof(context)).IsNotNull();
            jobResult.Verify(nameof(jobResult)).IsNotNull();
            jobResult.JobId.Verify(nameof(jobResult.JobId)).IsNotNull();

            context.Telemetry.ActivityStop(context, $"{nameof(UpdateCompletionList)} for JobId={((Guid)jobResult.JobId).ToString()}, Status={jobResult.Status}", (long)(jobResult.Duration?.TotalMilliseconds ?? 0));

            if (_semaphore != null)
            {
                _semaphore.Release();
                context.Telemetry.TrackMetric(context, $"{nameof(RunJobGraph)}:Release-Semaphore-CurrentCount", (double)_semaphore.CurrentCount);
            }

            lock (_lock)
            {
                if (_processedDict.TryGetValue((Guid)jobResult.JobId, out TKey value))
                {
                    if (jobResult.Status == JobStatus.Completed)
                    {
                        _graphContext.AddProcessedNodeKey(value);
                    }
                    else
                    {
                        _graphContext.AddStopNodeKey(value);
                    }

                    _runningKeys.Remove(value);
                }
            }
        }
    }
}
