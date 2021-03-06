﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageNet.Management
{
    public interface IRouteManager
    {
        Task<RouteRegistrationResponse> Register(IWorkContext context, RouteRegistrationRequest request);

        Task<IReadOnlyList<RouteLookupResponse>> Search(IWorkContext context, RouteLookupRequest request);

        Task Unregister(IWorkContext context, RouteRegistrationRequest routeRegistrationRequest);

        Task Clear(IWorkContext workContext);
    }
}