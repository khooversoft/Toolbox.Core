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
    internal class LoadAssemblyAction : IAction
    {
        private readonly IOption _option;

        public LoadAssemblyAction(IOption option)
        {
            _option = option;
        }

        public Task Run(IWorkContext context, IExecutionContext executionContext)
        {
            context.Verify(nameof(context)).IsNotNull();
            executionContext.Verify(nameof(executionContext)).IsNotNull();

            Assembly assembly = LoadAssembly(context);
            var list = new List<Function>().ToList();

            foreach (TypeInfo typeInfo in assembly.ExportedTypes)
            {
                context.Telemetry.Verbose(context, $"Searching type {typeInfo.Name}");
                foreach (MethodInfo methodInfo in typeInfo.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    context.Telemetry.Verbose(context, $"Searching method {methodInfo.Name} in type {typeInfo.Name}");

                    FunctionAttribute? functionAttribute = methodInfo.GetCustomAttribute<FunctionAttribute>();
                    if (functionAttribute != null)
                    {
                        context.Telemetry.Info(context, $"Found function {methodInfo.Name} : {functionAttribute.ToString()}");
                        list.Add(new Function(methodInfo, functionAttribute));
                    }
                }
            }

            executionContext.SetFunctions(list);

            return Task.CompletedTask;
        }

        private Assembly LoadAssembly(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();

            context.Telemetry.Info(context, $"Loading assembly {_option.AssemblyPath}");
            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(_option.AssemblyPath!);
            return assembly;
        }
    }
}
