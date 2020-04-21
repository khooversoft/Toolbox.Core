// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class InMemoryEventLogger : ITelemetryLogger, ITelemetryQuery
    {
        private readonly RingQueue<TelemetryMessage> _messages;

        public InMemoryEventLogger(int maxSize)
        {
            Verify.Assert(maxSize >= 0, $"{nameof(maxSize)} {maxSize} must be greater then zero");

            _messages = new RingQueue<TelemetryMessage>(maxSize);
        }

        public int MaxSize { get; }

        public int Count => _messages.Count;

        public void Write(TelemetryMessage message)
        {
            _messages.Enqueue(message);
        }

        public IReadOnlyList<TelemetryMessage> Query(Func<TelemetryMessage, bool> filter, int? firstCount = null, int? lastCount = null)
        {
            filter.VerifyNotNull(nameof(filter));

            TelemetryMessage[] data = _messages.ToArray();

            var filteredData = data
                .Where(x => filter(x))
                .ToArray();

            if (firstCount == null && lastCount == null)
            {
                return filteredData;
            }

            return filteredData.HeadAndTail(firstCount ?? 0, lastCount ?? int.MaxValue).ToArray();
        }

        public IEnumerator<TelemetryMessage> GetEnumerator()
        {
            return _messages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _messages.GetEnumerator();
        }
    }
}
