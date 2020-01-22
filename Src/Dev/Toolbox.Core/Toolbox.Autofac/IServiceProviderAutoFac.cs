using Autofac;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Autofac
{
    public interface IServiceProviderAutoFac : IServiceProviderProxy
    {
        ILifetimeScope BeginLifetimeScope(string tag);

        ILifetimeScope BeginLifetimeScope(string tag, Func<IEnumerable<Type>> configurationAction);
    }
}
