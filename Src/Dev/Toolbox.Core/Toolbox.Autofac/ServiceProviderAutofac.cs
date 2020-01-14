using Autofac;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Autofac
{
    public class ServiceProviderAutofac : IServiceProviderProxy
    {
        private readonly ILifetimeScope _container;

        public ServiceProviderAutofac(ILifetimeScope container)
        {
            container.Verify(nameof(container)).IsNotNull();

            _container = container;
        }

        public T BeginLifetimeScope<T>(string tag)
            where T : IDisposable
        {
            return (T)_container.BeginLifetimeScope(tag);
        }

        public T BeginLifetimeScope<T>(string tag, Func<IEnumerable<Type>> configurationAction)
            where T : IDisposable
        {
            configurationAction.Verify(nameof(configurationAction)).IsNotNull();

            return (T)_container.BeginLifetimeScope(tag, builder =>
            {
                configurationAction()
                    .ForEach(type => builder.RegisterType(type));
            });
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        public object GetServiceOptional(Type serviceType)
        {
            return _container.ResolveOptional(serviceType);
        }        
    }
}
