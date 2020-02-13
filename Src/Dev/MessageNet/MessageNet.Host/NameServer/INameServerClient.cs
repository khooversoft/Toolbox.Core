// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageNet.Host
{
    public interface INameServerClient
    {
        Task<bool> Ping(IWorkContext context);
        
        Task ClearAll(IWorkContext context);

        Task<RouteLookupResponse?> Lookup(IWorkContext context, RouteLookupRequest request);

        Task<RouteRegistrationResponse> Register(IWorkContext context, RouteRegistrationRequest request);

        Task Unregister(IWorkContext context, RouteRegistrationRequest request);
    }
}