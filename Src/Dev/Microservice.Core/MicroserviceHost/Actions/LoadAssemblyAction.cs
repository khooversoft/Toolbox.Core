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
            var list = new List<Function>();

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
                        list.Add(new Function(methodInfo, functionAttribute, GetParameterType(methodInfo)));
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

        public Type GetParameterType(MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();

            string methodName = $"method {methodInfo.Name} in class {methodInfo.DeclaringType!.FullName}";

            // Only two parameters are required
            parameters
                .Verify()
                .Assert(x => x.Length == 2, $"Function {methodName} does not have 2 parameters");

            // Verify first parameter is the "IWorkContext"
            parameters
                .First()
                .Verify()
                .Assert(x => x.ParameterType == typeof(IWorkContext), $"The first parameter is not {typeof(IWorkContext).GetType().FullName} for function {methodName}");

            // Figure out the second parameter's type, this must be a derived from RouteMessage<T>
            Type sendMessageType = parameters
                .Last()
                .Do(x => x.ParameterType)
                .Verify()
                .Assert(x => x.DeclaringType == typeof(RouteMessage<>), $"The second parameter type does not derived from RouteMessage<T> for function {methodName}")
                .Assert(x => x.GetConstructor(Type.EmptyTypes) != null, $"The second parameter type does not implement a parameterless constructor for function {methodName}")
                .Value;

            return sendMessageType;
        }
    }
}
