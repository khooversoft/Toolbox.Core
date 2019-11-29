// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System.Threading.Tasks;

namespace Khooversoft.MessageHub.Management
{
    public interface INodeRegistrationActor : IActor
    {
        Task Set(IWorkContext context, NodeRegistrationModel nodeRegistrationModel);

        Task Remove(IWorkContext context);

        Task<NodeRegistrationModel?> Get(IWorkContext context);
    }
}
