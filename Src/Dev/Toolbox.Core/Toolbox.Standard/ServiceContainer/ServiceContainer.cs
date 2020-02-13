// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class ServiceContainer : IServiceContainer
    {
        private readonly Func<Type, object>? _getService;
        private readonly Func<Type, object>? _getServiceOptional;
        private readonly Func<string, IEnumerable<Type>, IDisposable>? _createScope;

        public ServiceContainer(Func<Type, object>? getService, Func<Type, object>? getServiceOptional, Func<string, IEnumerable<Type>, IDisposable>? createScope)
        {
            _getService = getService;
            _getServiceOptional = getServiceOptional;
            _createScope = createScope;
        }

        public object GetService(Type serviceType) => _getService?.Invoke(serviceType) ?? throw new NotImplementedException($"Service was not setup");

        public object GetServiceOptional(Type serviceType) => _getServiceOptional?.Invoke(serviceType) ?? throw new NotImplementedException($"Service optional was not setup");

        public IDisposable CreateScope(string tag, IEnumerable<Type> types) => _createScope?.Invoke(tag, types) ?? throw new NotImplementedException("CreateScope was not setup");
    }
}
