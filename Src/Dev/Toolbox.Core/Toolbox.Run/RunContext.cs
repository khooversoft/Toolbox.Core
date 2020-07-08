using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Khooversoft.Toolbox.Run
{
    public class RunContext : IRunContext
    {
        public RunContext() { }

        public IProperty Property { get; } = new Property();
    }
}