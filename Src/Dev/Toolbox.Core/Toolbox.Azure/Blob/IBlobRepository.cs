// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public interface IBlobRepository
    {
        Task CreateContainer(IWorkContext context);

        Task DeleteContainer(IWorkContext context);

        Task<bool> Exists(IWorkContext context);

        Task Set(IWorkContext context, string path, string data);

        Task Set<T>(IWorkContext context, string path, T data) where T : class;

        Task<string> Get(IWorkContext context, string path);

        Task<T> Get<T>(IWorkContext context, string path) where T : class;

        Task Delete(IWorkContext context, string path);

        Task<IReadOnlyList<string>> List(IWorkContext context, string search);

        Task ClearAll(IWorkContext context);
    }
}
