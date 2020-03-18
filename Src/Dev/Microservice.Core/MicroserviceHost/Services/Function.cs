using Khooversoft.MessageNet.Client;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using System;
using System.Reflection;

namespace MicroserviceHost
{
    public class Function
    {
        public Function(MethodInfo method, MessageFunctionAttribute functionAttribute, Type messageType)
        {
            method.Verify(nameof(method)).IsNotNull();
            functionAttribute.Verify(nameof(functionAttribute)).IsNotNull();
            messageType.Verify(nameof(messageType)).IsNotNull();

            MethodInfo = method;
            FunctionAttribute = functionAttribute;
            MessageType = messageType;
        }

        public string Name => FunctionAttribute.Name;

        public MethodInfo MethodInfo { get; }

        public MessageFunctionAttribute FunctionAttribute { get; }

        public Type MessageType { get; }
    }
}