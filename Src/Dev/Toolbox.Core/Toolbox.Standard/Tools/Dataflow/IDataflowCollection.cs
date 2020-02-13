// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

namespace Khooversoft.Toolbox.Standard
{
    public interface IDataflowCollection<T>
    {
        void Add(IDataflow<T> targetBlock);
    }
}