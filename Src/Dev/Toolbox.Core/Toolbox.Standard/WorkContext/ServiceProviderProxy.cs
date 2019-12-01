// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Toolbox.Standard
{
    public class ServiceProviderProxy : ServiceProviderProxySimple, IServiceProviderProxy
    {
        private readonly Func<Type, object> _getServiceOptional;

        public ServiceProviderProxy(Func<Type, object> getService, Func<Type, object> getServiceOptional)
            :base(getService)
        {
            getService.Verify(nameof(getServiceOptional)).IsNotNull();

            _getServiceOptional = getServiceOptional;
        }

        public object GetServiceOptional(Type serviceType)
        {
            serviceType.Verify(nameof(serviceType)).IsNotNull();

            return _getServiceOptional(serviceType);
        }
    }
}
