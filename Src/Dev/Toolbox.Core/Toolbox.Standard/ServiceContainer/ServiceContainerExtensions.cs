// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Toolbox.Standard
{
    public static class ServiceContainerExtensions
    {
        public static T Resolve<T>(this IServiceProvider service)
        {
            return (T)service.GetService(typeof(T));
        }
    }
}
