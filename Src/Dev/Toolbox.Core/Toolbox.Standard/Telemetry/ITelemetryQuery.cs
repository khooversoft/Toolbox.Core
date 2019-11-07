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
