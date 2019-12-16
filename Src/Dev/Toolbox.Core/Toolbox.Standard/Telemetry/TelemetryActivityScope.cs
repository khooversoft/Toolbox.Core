// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.Standard
{
    public class TelemetryActivityScope : IDisposable
    {
        private IWorkContext? _workContext;

        public TelemetryActivityScope(IWorkContext context, string message)
        {
            context.Verify(nameof(context)).IsNotNull();
            message.Verify(nameof(message)).IsNotEmpty();

            _workContext = context;
            Message = message;
            StartTime = DateTimeOffset.Now;

            context.Telemetry.ActivityStart(context, Message);
        }

        public string Message { get; }

        public DateTimeOffset StartTime { get; }

        public void Dispose()
        {
            IWorkContext? context = Interlocked.Exchange(ref _workContext, null);
            context?.Telemetry.ActivityStop(context, Message, (long)(DateTimeOffset.Now - StartTime).TotalMilliseconds);
        }
    }
}
