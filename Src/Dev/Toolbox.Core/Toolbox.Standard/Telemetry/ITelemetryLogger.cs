using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public interface ITelemetryLogger
    {
        void Write(TelemetryMessage message);
    }
}
