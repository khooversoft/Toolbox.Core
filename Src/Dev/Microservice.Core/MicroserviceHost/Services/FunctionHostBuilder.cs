﻿using Autofac;
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
    public class FunctionHostBuilder
    {
        private readonly List<Type> _functionTypes = new List<Type>();

        public FunctionHostBuilder() { }

        public IReadOnlyList<Type> FunctionTypes => _functionTypes;

        public IServiceProviderProxy? Container { get; set; }

        public IPropertyResolver? PropertyResolver { get; set; }

        public IMessageNetClient? MessageNetClient { get; set; }

        public FunctionHostBuilder AddFunctionType(params Type[] types)
        {
            _functionTypes.AddRange(types);
            return this;
        }

        public FunctionHostBuilder UseContainer(IServiceProviderProxy serviceProviderProxy)
        {
            serviceProviderProxy.Verify(nameof(serviceProviderProxy)).IsNotNull();

            Container = serviceProviderProxy;
            return this;
        }

        public FunctionHostBuilder UseResolver(IPropertyResolver resolver)
        {
            resolver.Verify(nameof(resolver)).IsNotNull();

            PropertyResolver = resolver;
            return this;
        }
        public FunctionHostBuilder SetMessageNetClient(IMessageNetClient messageNetClient)
        {
            messageNetClient.Verify(nameof(messageNetClient)).IsNotNull();

            MessageNetClient = messageNetClient;
            return this;
        }

        public FunctionHost Build(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            MessageNetClient.Verify(nameof(MessageNetClient)).IsNotNull("Message net client is required");

            PropertyResolver ??= new PropertyResolver();

            IReadOnlyList<FunctionConfiguration> functionConfigurations = FunctionTypes
                .Select(x => GetFunctionMethods(x))
                .Select(x => GetFunctionConfiguration(x, PropertyResolver))
                .ToList();

            ILifetimeScope? lifetimeScope = null;

            if (context.Container != null)
            {
                lifetimeScope = context.Container.BeginLifetimeScope<ILifetimeScope>(nameof(FunctionHost), () => functionConfigurations.Select(x => x.Function.MethodInfo.DeclaringType!).ToList());
            }

            return new FunctionHost(functionConfigurations, MessageNetClient!, lifetimeScope);
        }

        /// <summary>
        /// Get function definitions for types
        /// </summary>
        /// <param name="types">type to scan</param>
        /// <returns>list of functions</returns>
        private static IReadOnlyList<Function> GetFunctionMethods(IEnumerable<Type> types)
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
        /// <param name="resolver"></param>
        /// <returns></returns>
        private static IReadOnlyList<FunctionConfiguration> GetFunctionConfiguration(IEnumerable<Function> functions, IPropertyResolver resolver)
        {
            functions.Verify(nameof(functions)).IsNotNull();
            resolver.Verify(nameof(resolver)).IsNotNull();

            return functions
                .Select(x => new FunctionConfiguration(x, getMessageNetNodeId(x, resolver)))
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
    }
}