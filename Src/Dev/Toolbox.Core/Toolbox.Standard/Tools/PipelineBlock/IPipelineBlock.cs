// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public interface IPipelineBlock<T>
    {
        Task Completion { get; }
        ISourceBlock<T> Current { get; }
        IDataflowBlock Root { get; }
        int Count { get; }

        IPipelineBlock<T> Add(IDataflowBlock source);
        IPipelineBlock<T> Add(IPipelineBlock<T> source);
        void Complete();
        Task<bool> Send(T value);
        bool Post(T value);
    }
}