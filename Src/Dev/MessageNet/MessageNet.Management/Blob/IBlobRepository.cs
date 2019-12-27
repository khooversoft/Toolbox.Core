// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageNet.Management
{
    public interface IBlobRepository
    {
        Task CreateContainer(IWorkContext context);

        Task Delete(IWorkContext context, string path);

        Task DeleteContainer(IWorkContext context);

        Task<string?> Get(IWorkContext context, string path);

        Task<IReadOnlyList<string>> List(IWorkContext context, string search);

        Task Set(IWorkContext context, string path, string data);

        Task ClearAll(IWorkContext context);
    }
}