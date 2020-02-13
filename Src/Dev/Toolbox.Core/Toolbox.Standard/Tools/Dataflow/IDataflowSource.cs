// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public interface IDataflowSource<T>
    {
        Task<bool> PostAsync(T message);

        bool Post(T message);

        void Complete();

        Task Completion { get; }
    }
}
