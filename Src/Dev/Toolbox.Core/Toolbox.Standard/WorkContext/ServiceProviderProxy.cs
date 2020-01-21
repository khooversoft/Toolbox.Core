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
            _getService = getService;
        }

        public ServiceProviderProxy(Func<Type, object> getService, Func<Type, object> getServiceOptional)
        {
            _getService = getService;
            _getServiceOptional = getServiceOptional;
        }

        public object GetService(Type serviceType) => _getService(serviceType);

        public object GetServiceOptional(Type serviceType) => _getServiceOptional?.Invoke(serviceType) ?? throw new ArgumentNullException($"Service optional was not setup");
    }
}
