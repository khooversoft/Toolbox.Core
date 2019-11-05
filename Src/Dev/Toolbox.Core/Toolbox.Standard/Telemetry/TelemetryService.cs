using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public class TelemetryService
    {
        //private ConsoleEventLogger _consoleLogger;
        //private FileEventLogger _fileLogger;
        //private FileEventLogger _fileEventLogger;
        //private InMemoryEventLogger _inMemoryLogger;
        //private ITelemetrySecretManager _telemetrySecretManager;

        private readonly BroadcastBlock<TelemetryMessage> _broadcastBlock;
        private Func<TelemetryMessage, TelemetryMessage>? _policy;

        public TelemetryService()
        {
            //_broadcastBlock = new BroadcastBlock<TelemetryMessage>(x => x)
            //    .LinkTo(new TransformBlock<TelemetryMessage, TelemetryMessage>(x => x)
            //    .RouteTo(x => )
        }

        public TelemetryService SetPolicy(Func<TelemetryMessage, TelemetryMessage>? policy)
        {
            _policy = policy;
            return this;
        }
    }
}
