using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    internal class LoadAssemblyActivity
    {
        private readonly IOption _option;

        public LoadAssemblyActivity(IOption option)
        {
            _option = option;
        }

        public Task Load(IWorkContext context, IExecutionContext executionContext)
        {
            context.VerifyNotNull(nameof(context));
            executionContext.VerifyNotNull(nameof(executionContext));

            executionContext.FunctionInfos = LoadAssembly(context)
                .FindMethodsByAttribute<MessageFunctionAttribute>()
                .ToList();

            return Task.CompletedTask;
        }

        private Assembly LoadAssembly(IWorkContext context)
        {
            context.VerifyNotNull(nameof(context));

            context.Telemetry.Info(context, $"Loading assembly {_option.AssemblyPath}");
            Assembly assembly = Reflection.LoadFromAssemblyPath(_option.AssemblyPath!);
            return assembly;
        }
    }
}
