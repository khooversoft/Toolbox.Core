// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Execution context
    /// </summary>
    public interface IWorkContext
    {
        CorrelationVector Cv { get; }
        StringVector Tag { get; }
        IServiceProvider? Container { get; }
        CancellationToken CancellationToken { get; }
        ITelemetry Telemetry { get; }
        IEventDimensions Dimensions { get; }

        IWorkContext WithNewCv();
        IWorkContext WithExtended();
        IWorkContext WithIncrement();
        IWorkContext WithTag(StringVector tag, [CallerMemberName] string? memberName = null);
        IWorkContext WithMethodName([CallerMemberName] string? memberName = null);
        IWorkContext With(ITelemetry eventLog);
        IWorkContext With(IEventDimensions eventDimenensions);

        WorkContextBuilder ToBuilder();
    }
}
