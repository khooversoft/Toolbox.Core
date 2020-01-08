using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using System;
using System.Reflection;

namespace MicroserviceHost
{
    internal class Function
    {
        public Function(MethodInfo method, FunctionAttribute functionAttribute)
        {
            Method = method;
            FunctionAttribute = functionAttribute;
        }

        public string Name => FunctionAttribute.Name;

        public MethodInfo Method { get; private set; }

        public FunctionAttribute FunctionAttribute { get; private set; }

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