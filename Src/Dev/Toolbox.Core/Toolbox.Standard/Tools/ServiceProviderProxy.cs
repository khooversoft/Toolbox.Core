// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class ServiceProviderProxy : IServiceProvider
    {
        private readonly Func<Type, object> _getService;

        public ServiceProviderProxy(Func<Type, object> getService)
        {
            _getService = getService;
        }

        public object GetService(Type serviceType)
        {
            return _getService(serviceType);
        }
    }
}
