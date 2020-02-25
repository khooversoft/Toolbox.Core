// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageNet.Management
{
    public interface IRouteRepository
    {
        Task<QueueId> Register(IWorkContext context, RouteRequest request);

        Task<IReadOnlyList<QueueId>> Search(IWorkContext context, RouteRequest request);

        Task Unregister(IWorkContext context, RouteRequest routeRegistrationRequest);

        Task Clear(IWorkContext workContext);
    }
}