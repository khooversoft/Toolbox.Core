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
            method.VerifyNotNull(nameof(method));
            functionAttribute.VerifyNotNull(nameof(functionAttribute));
            messageType.VerifyNotNull(nameof(messageType));

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