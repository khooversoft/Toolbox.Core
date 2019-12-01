// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class ServiceProviderProxySimple : IServiceProvider
    {
        private readonly Func<Type, object> _getService;

        public ServiceProviderProxySimple(Func<Type, object> getService)
        {
            getService.Verify(nameof(getService)).IsNotNull();

            _getService = getService;
        }

        public object GetService(Type serviceType)
        {
            serviceType.Verify(nameof(serviceType)).IsNotNull();

            return _getService(serviceType);
        }
    }
}
