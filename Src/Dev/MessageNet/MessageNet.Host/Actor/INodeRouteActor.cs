// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    internal interface INodeRouteActor : IActor
    {
        Task<NodeRegistration?> Lookup(IWorkContext context);

        Task<NodeRegistration> Register(IWorkContext context);
    }
}