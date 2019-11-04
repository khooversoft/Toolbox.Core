// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public struct MetricSample
    {
        public MetricSample(float value, TimeSpan span, int count)
        {
            Recorded = DateTimeOffset.Now;
            Span = span;
            Value = value;
            Count = count;
        }

        public DateTimeOffset Recorded { get; }

        public TimeSpan Span { get; }

        public float Value { get; }

        public int Count { get; }

        public float Tps => Count == 0 || Span.TotalSeconds == 0 ? 0 : Count / (float)Span.TotalSeconds;

        public override string ToString()
        {
            return $"Recorded:{Recorded.ToString("MM/dd/yyyyTHH:mm:ss.FFFF")}, Span:{Span}, Value: {Value}, Count: {Count}, Tps:{Tps}";
        }
    }

    public class MetricSampler
    {
        private readonly IQueueSizePolicy<MetricSample> _events = new Queue<MetricSample>().SetFixSizePolicy(100);
        private readonly TimeSpan _sampleRate;
        private readonly ActionBlock<MetricSample> _actionBlock;

        private Collector _currentProbe;
        private Collector _workProbe;
        private DateTimeOffset _lastSample;
        private int _reduceIsRunning = 0;
        private bool _isRunning = false;
        private bool _isStopped = false;

        private readonly object _lock = new object();
        private readonly object _lockEvents = new object();

        public MetricSampler(TimeSpan sampleRate)
        {
            _sampleRate = sampleRate;

            _actionBlock = new ActionBlock<MetricSample>(ProcessEvent);
            _currentProbe = new Collector();
            _workProbe = new Collector();
        }

        public MetricSampler Start()
        {
            _isStopped.Verify().Assert(x => x == false, "Sampler has been stopped");

            lock (_lock)
            {
                _lastSample = DateTimeOffset.Now;
                _events.Clear();
                _isRunning = true;
                _currentProbe.Reset();
                _workProbe.Reset();
            }

            return this;
        }

        public MetricSampler Add(float value)
        {
            _isRunning.Verify().Assert(x => x == true, "Sampler is not running");

            lock (_lock)
            {
                _currentProbe.Add(value);
                if (_lastSample + _sampleRate > DateTimeOffset.Now) return this;
            }

            Sample();

            return this;
        }

        public MetricSampler Stop()
        {
            lock (_lock)
            {
                _isRunning = false;
                _isStopped = true;
                Sample();
                _actionBlock.Complete();
                _actionBlock.Completion.Wait();
            }

            return this;
        }

        public IReadOnlyList<MetricSample> GetMetrics(bool clear = false)
        {
            lock (_lock)
            {
                var list = _events.ToList();
                if (clear) _events.Clear();
                return list;
            }
        }

        private void Sample()
        {
            int currentIsRunning = Interlocked.CompareExchange(ref _reduceIsRunning, 1, 0);
            if (currentIsRunning != 0) return;

            try
            {
                _workProbe.Reset();
                _workProbe = Interlocked.Exchange(ref _currentProbe, _workProbe);

                _actionBlock.Post(new MetricSample(_workProbe.Value, DateTimeOffset.Now - _workProbe.StartDate, _workProbe.Count));
                _lastSample = DateTimeOffset.Now;
            }
            finally
            {
                Interlocked.CompareExchange(ref _reduceIsRunning, 0, 1)
                    .Verify()
                    .Assert(x => x == 1, "Running tracking state is invalid");
            }
        }

        private void ProcessEvent(MetricSample metricSample)
        {
            lock (_lockEvents)
            {
                _events.Enqueue(metricSample);
            }
        }

        private class Collector
        {
            private readonly object _lock = new object();

            public Collector()
            {
                StartDate = DateTimeOffset.Now;
            }

            public float Value { get; private set; }
            public int Count { get; private set; }
            public DateTimeOffset StartDate { get; private set; }

            public void Add(float value)
            {
                lock (_lock)
                {
                    Value += value;
                    Count++;
                }
            }

            public void Reset()
            {
                lock (_lock)
                {
                    StartDate = DateTimeOffset.Now;
                    Value = 0;
                    Count = 0;
                }
            }

            public TimeSpan GetSpan() => DateTimeOffset.Now - StartDate;
        }
    }
}
