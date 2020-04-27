using Autofac;
using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Autofac;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroserviceHost
{
    public class FunctionHostBuilder
    {
        private readonly List<Type> _functionTypes = new List<Type>();

        public FunctionHostBuilder() { }

        public IReadOnlyList<Type> FunctionTypes => _functionTypes;

        public IServiceContainer? Container { get; set; }

        public IPropertyResolver? PropertyResolver { get; set; }

        public IMessageNetConfig? MessageNetConfig { get; set; }

        public FunctionHostBuilder AddFunctionType(params Type[] types)
        {
            _functionTypes.AddRange(types);
            return this;
        }

        public FunctionHostBuilder UseContainer(IServiceContainer serviceContainer)
        {
            serviceContainer.VerifyNotNull(nameof(serviceContainer));

            Container = serviceContainer;
            return this;
        }

        public FunctionHostBuilder UseResolver(IPropertyResolver resolver)
        {
            resolver.VerifyNotNull(nameof(resolver));

            PropertyResolver = resolver;
            return this;
        }

        public FunctionHostBuilder SetMessageNetConfig(IMessageNetConfig messageNetConfig)
        {
            MessageNetConfig = messageNetConfig;
            return this;
        }

        public FunctionHost Build(IWorkContext context)
        {
            context.VerifyNotNull(nameof(context));
            MessageNetConfig.VerifyNotNull(nameof(MessageNetConfig));

            PropertyResolver ??= new PropertyResolver();

            IReadOnlyList<FunctionConfiguration> functionConfigurations = FunctionTypes
                .Func(x => GetFunctionMethods(x))
                .Func(x => GetFunctionConfiguration(x, PropertyResolver))
                .ToList();

            ILifetimeScope? lifetimeScope = null;

            if (context.Container != null)
            {
                lifetimeScope = context.Container.BeginLifetimeScope(nameof(FunctionHost), functionConfigurations.Select(x => x.Function.MethodInfo.DeclaringType!));
            }

            return new FunctionHost(functionConfigurations, MessageNetConfig, lifetimeScope);
        }

        /// <summary>
        /// Get function definitions for types
        /// </summary>
        /// <param name="types">type to scan</param>
        /// <returns>list of functions</returns>
        private static IReadOnlyList<Function> GetFunctionMethods(IEnumerable<Type> types)
        {
            types.VerifyNotNull(nameof(types));

            List<Function> results = types
                .SelectMany(x => x.GetMethodsWithAttribute<MessageFunctionAttribute>())
                .Select(x => new Function(x.MethodInfo, x.Attribute, getParameterType(x.MethodInfo)))
                .ToList();

            return results;


            static Type getParameterType(MethodInfo methodInfo)
            {
                ParameterInfo[] parameters = methodInfo.GetParameters();

                string methodName = $"method {methodInfo.Name} in class {methodInfo.DeclaringType!.FullName}";

                // Only two parameters are required
                parameters
                    .VerifyAssert(x => x.Length == 2, $"Function {methodName} does not have 2 parameters");

                // Verify first parameter is the "IWorkContext"
                parameters
                    .First()
                    .VerifyAssert(x => x.ParameterType == typeof(IWorkContext), $"The first parameter is not {typeof(IWorkContext).GetType().FullName} for function {methodName}");

                // Figure out the second parameter's type, this must be a derived from RouteMessage<T>
                Type sendMessageType = parameters
                    .Last()
                    .Func(x => x.ParameterType);

                return sendMessageType;
            }
        }

        /// <summary>
        /// Get function configuration
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        private static IReadOnlyList<FunctionConfiguration> GetFunctionConfiguration(IEnumerable<Function> functions, IPropertyResolver resolver)
        {
            functions.VerifyNotNull(nameof(functions));
            resolver.VerifyNotNull(nameof(resolver));

            return functions
                .Select(x => new FunctionConfiguration(x, getMessageNetNodeId(x, resolver)))
                .ToList();

            static string getMessageNetNodeId(Function function, IPropertyResolver resolver)
            {
                return function.FunctionAttribute.NodeId
                    .Resolve(resolver)
                    .VerifyNotEmpty($"Function: {function.Name} {function.FunctionAttribute.NodeId} is required")
                    .VerifyAssert(x => x.GetPropertyNames()?.Count == 0, $"Unresolved properties for {function.FunctionAttribute.NodeId}");
            }
        }
    }
}
