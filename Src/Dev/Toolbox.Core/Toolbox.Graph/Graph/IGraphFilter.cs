// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace KHooversoft.Toolbox.Graph
{
    public interface IGraphFilter<TKey>
    {
        IReadOnlyList<TKey> IncludeNodeKeys { get; }

        IReadOnlyList<TKey> ExcludeNodeKeys { get; }

        bool IncludeLinkedNodes { get; }

        bool IncludeDependentNodes { get; }
    }
}