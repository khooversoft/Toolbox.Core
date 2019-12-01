// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.MessageHub.Interface;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageHub.Management
{
    public interface IRouteManager
    {
        Task<RouteRegistrationResponse> Register(IWorkContext context, RouteRegistrationRequest request);
        Task<IReadOnlyList<RouteLookupResponse>> Search(IWorkContext context, RouteLookupRequest request);
        Task Unregister(IWorkContext context, string nodeId);
    }
}