using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class ServiceProviderProxy : IServiceProviderProxy
    {
        private readonly Func<Type, object> _getService;
        private readonly Func<Type, object>? _getServiceOptional;

        public ServiceProviderProxy(Func<Type, object> getService)
        {
            getService.Verify(nameof(getService)).IsNotNull();

            _getService = getService;
        }

        public ServiceProviderProxy(Func<Type, object> getService, Func<Type, object> getServiceOptional)
            : this(getService)
        {
            getServiceOptional.Verify(nameof(getServiceOptional)).IsNotNull();

            _getServiceOptional = getServiceOptional;
        }

        public object GetService(Type serviceType) => _getService(serviceType);

        public object GetServiceOptional(Type serviceType) => _getServiceOptional?.Invoke(serviceType) ?? throw new ArgumentNullException($"Service optional was not setup");
    }
}
