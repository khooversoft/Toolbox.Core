using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Interface
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class MessageFunctionAttribute : Attribute
    {
        // This is a positional argument
        public MessageFunctionAttribute(string name, string nodeId)
        {
            Name = name;
            NodeId = nodeId;
        }

        public string Name { get; }

        public string NodeId { get; }
    }
}
