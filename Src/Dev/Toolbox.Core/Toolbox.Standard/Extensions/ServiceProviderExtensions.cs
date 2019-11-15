// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class ServiceProviderExtensions
    {
        public static T Resolve<T>(this IServiceProvider service)
        {
            return (T)service.GetService(typeof(T));
        }
    }
}
