using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Interface
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class FunctionAttribute : Attribute
    {
        // This is a positional argument
        public FunctionAttribute(string name, string inputUri)
        {
            Name = name;
            NodeId = inputUri;
        }

        public string Name { get; }

        public string NodeId { get; }

        public override string ToString()
        {
            return Name + ":" + NodeId;
        }
    }
}
