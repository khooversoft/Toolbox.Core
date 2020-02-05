// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public class TelemetryService : ITelemetryService, IDisposable
    {
        private IDataflowSource<TelemetryMessage> _dataflow;

        public TelemetryService(IDataflowSource<TelemetryMessage> dataflow)
        {
            _dataflow = dataflow;
        }

        public ITelemetry CreateLogger(string eventSourceName)
        {
            eventSourceName.Verify(nameof(eventSourceName)).IsNotEmpty();

            return new LoggerProxy(this, eventSourceName);
        }

        public void Write(TelemetryMessage message)
        {
            message.Verify(nameof(message)).IsNotNull();

            _dataflow.Post(message);
        }

        public void Dispose()
        {
            var dataflow = Interlocked.Exchange(ref _dataflow, null!);

            dataflow?.Complete();
            dataflow?.Completion?.Wait();
        }
    }
}
