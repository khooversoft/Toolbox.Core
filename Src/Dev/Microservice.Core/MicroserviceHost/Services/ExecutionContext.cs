using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroserviceHost
{
    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext()
        {
        }

        public IReadOnlyList<Function> Functions { get; private set; } = new List<Function>();

        public void SetFunctions(IEnumerable<Function> functions)
        {
            Functions = functions.ToList();
        }
    }
}
