// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Management
{
    public interface IRegisterStore
    {
        Task Set(IWorkContext context, string path, NodeRegistrationModel nodeRegistrationModel);

        Task Remove(IWorkContext context, string path);

        Task<NodeRegistrationModel?> Get(IWorkContext context, string path);

        Task<IReadOnlyList<NodeRegistrationModel>> List(IWorkContext context, string search);

        Task ClearAll(IWorkContext context);
    }
}