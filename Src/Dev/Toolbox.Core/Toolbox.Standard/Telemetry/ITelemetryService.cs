using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard.Telemetry
{
    public interface ITelemetryService
    {
        ITelemetry CreateLogger(string eventSourceName);

        void Post(TelemetryMessage message);
    }
}
