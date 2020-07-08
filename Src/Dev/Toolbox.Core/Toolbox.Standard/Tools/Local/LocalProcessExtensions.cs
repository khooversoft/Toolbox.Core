using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class LocalProcessExtensions
    {
        public static MonitorLocalProcess Build(this LocalProcessBuilder builder, Func<string, MonitorState?> monitor, ILogger logger) => new MonitorLocalProcess(builder, monitor, logger);
    }
}
