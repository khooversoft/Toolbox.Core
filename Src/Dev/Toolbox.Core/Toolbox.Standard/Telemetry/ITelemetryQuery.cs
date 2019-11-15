// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public interface ITelemetryQuery : IEnumerable<TelemetryMessage>
    {
        IReadOnlyList<TelemetryMessage> Query(Func<TelemetryMessage, bool> filter, int? firstCount = null, int? lastCount = null);
    }
}
