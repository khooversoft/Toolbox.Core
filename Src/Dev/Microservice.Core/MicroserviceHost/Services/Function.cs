using Khooversoft.MessageNet.Client;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using System;
using System.Reflection;

namespace MicroserviceHost
{
    internal class Function
    {
        public Function(MethodInfo method, FunctionAttribute functionAttribute, Type messageType)
        {
            method.Verify(nameof(method)).IsNotNull();
            functionAttribute.Verify(nameof(functionAttribute)).IsNotNull();
            messageType.Verify(nameof(messageType)).IsNotNull();

            MethodInfo = method;
            FunctionAttribute = functionAttribute;
            MessageType = messageType;
        }

        public string Name => FunctionAttribute.Name;

        public MethodInfo MethodInfo { get; private set; }

        public FunctionAttribute FunctionAttribute { get; private set; }

        public Type MessageType { get; }

        public IMessageNetClient? MessageNetClient { get; private set; }

        public void SetMessageNetClient(IMessageNetClient messageNetClient)
        {
            MessageNetClient = messageNetClient
                .Verify(nameof(messageNetClient))
                .IsNotNull()
                .Value;
        }
    }
}