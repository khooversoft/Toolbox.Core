using Khooversoft.MessageNet.Interface;
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

        public string FunctionName => FunctionAttribute.Name;

        public MethodInfo Method { get; private set; }

        public FunctionAttribute FunctionAttribute { get; private set; }

        public IMessageNetClient? MessageNetClient { get; private set; }
    }
}