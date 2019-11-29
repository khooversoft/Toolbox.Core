// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public interface IStateManager
    {
        IReadOnlyList<IStateItem> StateItems { get; }

        Task<bool> Set(IWorkContext context);

        Task<bool> Test(IWorkContext context);
    }
}
