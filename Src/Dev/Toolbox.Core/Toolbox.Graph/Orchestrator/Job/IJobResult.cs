// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace KHooversoft.Toolbox.Graph
{
    public interface IJobResult
    {
        Guid? JobId { get; }

        JobStatus Status { get; }

        AggregateException Exception { get; }

        IReadOnlyList<string> Errors { get; }

        TimeSpan? Duration { get; }
    }
}
