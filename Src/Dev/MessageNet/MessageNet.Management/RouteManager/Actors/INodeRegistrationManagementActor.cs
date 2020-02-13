// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.MessageNet.Management;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageNet.Management
{
    public interface INodeRegistrationManagementActor : IActor
    {
        Task<IReadOnlyList<NodeRegistrationModel>> List(IWorkContext context, string search);

        Task ClearRegistery(IWorkContext context);
    }
}