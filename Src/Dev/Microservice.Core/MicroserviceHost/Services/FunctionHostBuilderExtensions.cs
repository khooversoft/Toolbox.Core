using Autofac;
using Khooversoft.MessageNet.Client;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    public static class FunctionHostBuilderExtensions
    {
        /// <summary>
        /// Get function definitions for types
        /// </summary>
        /// <param name="types">type to scan</param>
        /// <returns>list of functions</returns>
        public static IReadOnlyList<Function> GetFunctionMethods(this IEnumerable<Type> types)
        {
            types.Verify(nameof(types)).IsNotNull();

            List<Function> results = types
                .SelectMany(x => x.GetMethodsWithAttribute<FunctionAttribute>())
                .Select(x => new Function(x.MethodInfo, x.Attribute, getParameterType(x.MethodInfo)))
                .ToList();

            return results;


            static Type getParameterType(MethodInfo methodInfo)
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
                    .Do(x => x.ParameterType);

                return sendMessageType;
            }
        }

        /// <summary>
        /// Get function configuration
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="nameServerUri"></param>
        /// <param name="serviceBusConnection"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static IReadOnlyList<FunctionConfiguration> GetFunctionConfiguration(this IEnumerable<Function> functions, Uri nameServerUri, string serviceBusConnection, IPropertyResolver resolver)
        {
            functions.Verify(nameof(functions)).IsNotNull();
            nameServerUri.Verify(nameof(nameServerUri)).IsNotNull();
            serviceBusConnection.Verify(nameof(serviceBusConnection)).IsNotEmpty();
            resolver.Verify(nameof(resolver)).IsNotNull();

            return functions
                .Select(x => new FunctionConfiguration(x, nameServerUri, serviceBusConnection, getMessageNetNodeId(x, resolver)))
                .ToList();


            static string getMessageNetNodeId(Function function, IPropertyResolver resolver)
            {
                return function.FunctionAttribute.NodeId
                    .Resolve(resolver)
                    .Verify(nameof(function.FunctionAttribute.NodeId))
                    .IsNotEmpty($"Function: {function.Name} {function.FunctionAttribute.NodeId} is required")
                    .Assert(x => x.GetPropertyNames()?.Count == 0, $"Unresolved properties for {function.FunctionAttribute.NodeId}")
                    .Value;
            }
        }

        /// <summary>
        /// Register function types with DI container
        /// </summary>
        /// <param name="functionConfigurations">list of function configurations</param>
        /// <param name="containerBuilder">DI container</param>
        /// <returns>list of function configuration</returns>
        public static IReadOnlyList<FunctionConfiguration> RegisterTypesWithContainer(this IReadOnlyList<FunctionConfiguration> functionConfigurations, Action<Type>? builder)
        {
            if (builder == null) return functionConfigurations;

            functionConfigurations
                .ForEach(x => builder(x.Function.MethodInfo.DeclaringType!));

            return functionConfigurations;
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="functionConfigurations"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="messageNetClient"></param>
        /// <returns></returns>
        public static async Task<IReadOnlyList<FunctionMessageReceiver>> Run(this IEnumerable<FunctionConfiguration> functionConfigurations, IWorkContext context, ILifetimeScope lifetimeScope, IMessageNetClient messageNetClient)
        {
            functionConfigurations.Verify(nameof(functionConfigurations)).IsNotNull();
            messageNetClient.Verify(nameof(messageNetClient)).IsNotNull();
            lifetimeScope.Verify(nameof(lifetimeScope)).IsNotNull();

            IReadOnlyList<FunctionMessageReceiver> functionMessageReceivers = functionConfigurations
                .Select(x => new FunctionMessageReceiver(x, lifetimeScope, messageNetClient))
                .ToList();

            await functionMessageReceivers
                .ForEachAsync(x => x.Start(context.WithNewCv()));

            return functionMessageReceivers;
        }

        /// <summary>
        /// Stop message receivers
        /// </summary>
        /// <param name="functionMessageReceivers">list of message receivers to stop</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IReadOnlyList<FunctionMessageReceiver>> Stop(this IReadOnlyList<FunctionMessageReceiver> functionMessageReceivers, IWorkContext context)
        {
            functionMessageReceivers.Verify(nameof(functionMessageReceivers)).IsNotNull();

            await functionMessageReceivers
                .ForEachAsync(x => x.Stop(context));

            return functionMessageReceivers;
        }
    }
}
