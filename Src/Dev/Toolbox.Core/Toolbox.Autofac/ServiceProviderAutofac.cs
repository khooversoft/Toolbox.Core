using Autofac;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Autofac
{
    public class ServiceProviderAutofac : ServiceProviderProxy, IServiceProviderAutoFac
    {
        private readonly Func<string, IEnumerable<Type>, ILifetimeScope> _getLifetimeScope;

        public ServiceProviderAutofac(Func<Type, object> getService, Func<Type, object> getServiceOptional, Func<string, IEnumerable<Type>, ILifetimeScope> getLifetimeScope)
            : base(getService, getServiceOptional)
        {
            getLifetimeScope.Verify(nameof(getLifetimeScope)).IsNotNull();

            _getLifetimeScope = getLifetimeScope;
        }

        public ILifetimeScope BeginLifetimeScope(string tag)
        {
            return _getLifetimeScope(tag, Enumerable.Empty<Type>());
        }

        public ILifetimeScope BeginLifetimeScope(string tag, Func<IEnumerable<Type>> configurationAction)
        {
            configurationAction.Verify(nameof(configurationAction)).IsNotNull();

            return _getLifetimeScope(tag, configurationAction());
        }
    }
}
