using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    public class FunctionConfiguration
    {
        public FunctionConfiguration(Function function, string nodeId)
        {
            Function = function;
            NodeId = nodeId;
        }

        public Function Function { get; }

        public string NodeId { get; }
    }
}
