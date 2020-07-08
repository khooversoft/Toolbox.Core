// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public interface IBlobRepository
    {
        Task ClearAll(CancellationToken token);
        Task CreateContainer(CancellationToken token);
        Task Delete(string path, CancellationToken token);
        Task DeleteContainer(CancellationToken token);
        Task<IReadOnlyList<byte>> Download(string path);
        Task<bool> Exists(CancellationToken token);
        Task<string> Get(string path);
        Task<T> Get<T>(string path) where T : class;
        Task<IReadOnlyList<string>> List(string search);
        bool SearchCompare(string search, string name);
        Task Set(string path, string data, CancellationToken token);
        Task Set<T>(string path, T data, CancellationToken token) where T : class;
        Task Upload(string path, IEnumerable<byte> content, CancellationToken token);
        Task Upload(string path, Stream content, CancellationToken token);
    }
}