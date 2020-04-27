using Autofac;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecutionContextBuilder
    {
        public ExecutionContextBuilder() { }

        public IPropertyResolver _propertyResolver { get; set; } = new PropertyResolver();

        public Uri? NameServerUri { get; set; }

        public string? ServiceBusConnection { get; set; }

        public Type[]? Types { get; set; }

        public ContainerBuilder? ContainerBuilder { get; set; }

        public ExecutionContextBuilder SetPropertyResolver(IPropertyResolver propertyResolver)
        {
            _propertyResolver.VerifyNotNull(nameof(_propertyResolver));

            _propertyResolver = propertyResolver;
            return this;
        }

        public ExecutionContextBuilder SetNameServerUri(Uri nameServerUri)
        {
            nameServerUri.VerifyNotNull(nameof(nameServerUri));

            NameServerUri = nameServerUri;
            return this;
        }

        public ExecutionContextBuilder SetServiceBusConnection(string serviceBusConnection)
        {
            serviceBusConnection.VerifyNotEmpty(nameof(serviceBusConnection));

            ServiceBusConnection = serviceBusConnection;
            return this;
        }

        public ExecutionContextBuilder SetTypes(params Type[] types)
        {
            types.VerifyNotNull(nameof(types));

            Types = types;
            return this;
        }

        public ExecutionContextBuilder SetContainer(ContainerBuilder container)
        {
            container.VerifyNotNull(nameof(container));

            ContainerBuilder = container;
            return this;
        }

        public IReadOnlyList<FunctionConfiguration> Build(IWorkContext context)
        {
            _propertyResolver.VerifyNotNull("Property resolver not set");
            NameServerUri.VerifyNotNull("Name server URI is not set");
            ServiceBusConnection.VerifyNotEmpty("Service bus connection is not set");

            IReadOnlyList<Function> functions = GetFunctions(context, Types!);
            IReadOnlyList<FunctionConfiguration> functionConfigurations = GetFunctionConfiguration(context, functions);

            if( ContainerBuilder != null)
            {
                functionConfigurations
                    .ForEach(x => ContainerBuilder.RegisterType(x.Function.MethodInfo.DeclaringType!));
            }

            return functionConfigurations;
        }

        private IReadOnlyList<Function> GetFunctions(IWorkContext context, Type[] types)
        {
            var list = new List<Function>();

            List<Function> results = types
                .SelectMany(x => x.GetMethodsWithAttribute<MessageFunctionAttribute>())
                .Select(x => new Function(x.MethodInfo, x.Attribute, GetParameterType(x.MethodInfo)))
                .ToList();

            return results;

            //foreach (TypeInfo typeInfo in types)
            //{
            //    context.Telemetry.Verbose(context, $"Searching type {typeInfo.Name}");
            //    foreach (MethodInfo methodInfo in typeInfo.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            //    {
            //        context.Telemetry.Verbose(context, $"Searching method {methodInfo.Name} in type {typeInfo.Name}");

            //        FunctionAttribute? functionAttribute = methodInfo.GetCustomAttribute<FunctionAttribute>();
            //        if (functionAttribute != null)
            //        {
            //            context.Telemetry.Info(context, $"Found function {methodInfo.Name} : {functionAttribute.ToString()}");
            //            list.Add(new Function(methodInfo, functionAttribute, GetParameterType(methodInfo)));
            //        }
            //    }
            //}

            //return list;
        }

        private Type GetParameterType(MethodInfo methodInfo)
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

        public IReadOnlyList<FunctionConfiguration> GetFunctionConfiguration(IWorkContext context, IReadOnlyList<Function> functions)
        {
            context.VerifyNotNull(nameof(context));
            functions.VerifyNotNull(nameof(functions));

            return functions
                .Select(x => MessageNetFactory(x))
                .ToList();
        }

        private FunctionConfiguration MessageNetFactory(Function function)
        {
            string nodeId = function.FunctionAttribute.NodeId
                .Resolve(_propertyResolver!)
                .VerifyNotEmpty($"Function: {function.Name} {function.FunctionAttribute.NodeId} is required")
                .VerifyAssert(x => x.GetPropertyNames()?.Count == 0, $"Unresolved properties for {function.FunctionAttribute.NodeId}");

            return new FunctionConfiguration(function, nodeId);
        }
    }
}
