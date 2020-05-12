using Autofac;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroserviceHost
{
    internal class ExecutionContext : IExecutionContext
    {
        public ILifetimeScope? LifetimeScope { get; set; }

        public IReadOnlyList<FunctionInfo>? FunctionInfos { get; set; }

        public IReadOnlyList<Type>? KnownInjectMethodTypes { get; set; }
    }
}
