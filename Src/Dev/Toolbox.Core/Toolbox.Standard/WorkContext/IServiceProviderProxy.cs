// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Toolbox.Standard
{
    public interface IServiceProviderProxy : IServiceProvider
    {
        object GetServiceOptional(Type serviceType);
    }
}