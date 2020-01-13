using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    public class FunctionConfiguration
    {
        public FunctionConfiguration(Function function, Uri nameSErverUri, string serviceBusConnection, string nodeId)
        {
            Function = function;
            NameServerUri = nameSErverUri;
            ServiceBusConnection = serviceBusConnection;
            NodeId = nodeId;
        }

        public Function Function { get; }

        public Uri NameServerUri { get; }

        public string ServiceBusConnection { get; }

        public string NodeId { get; }
    }
}
