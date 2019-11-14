﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public interface ITelemetryService : ITelemetryLogger
    {
        ITelemetry CreateLogger(string eventSourceName);
    }
}