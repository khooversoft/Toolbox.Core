using Khooversoft.MessageNet.Client;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using System;
using System.Reflection;

namespace MicroserviceHost
{
    public class Function
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

        public MethodInfo MethodInfo { get; }

        public FunctionAttribute FunctionAttribute { get; }

        public Type MessageType { get; }

        public IMessageNetClient? MessageNetClient { get; private set; }

        public string? NodeId { get; private set; }

        public void SetMessageNetClient(IMessageNetClient messageNetClient, string nodeId)
        {
            messageNetClient.Verify(nameof(messageNetClient)).IsNotNull();
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();

            MessageNetClient = messageNetClient;
            NodeId = nodeId;
        }
    }
}