// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public class DataflowSource<T> : IDataflowSource<T>, IDisposable
    {
        private IList<IDataflowBlock> _dataflowBlocks;
        private readonly ITargetBlock<T> _root;

        public DataflowSource(IEnumerable<IDataflowBlock> dataflowBlocks)
        {
            dataflowBlocks.VerifyNotNull(nameof(dataflowBlocks));

            _dataflowBlocks = dataflowBlocks.ToList();
            _dataflowBlocks.Count.VerifyAssert(x => x > 0, _ => $"{nameof(dataflowBlocks)} count must be greater than 0");

            _root = _dataflowBlocks.First() as ITargetBlock<T> ?? throw new ArgumentException($"First block must be implement ITargetBlock<T> interface");
        }

        public Task Completion => _dataflowBlocks
                    .Select(x => x.Completion)
                    .WhenAll();


        public void Complete() => _root.Complete();

        public Task<bool> PostAsync(T message) => _root.SendAsync(message);

        public bool Post(T message) => _root.Post(message);

        public void Dispose()
        {
            IList<IDataflowBlock> dataflowBlocks = Interlocked.Exchange(ref _dataflowBlocks, null!);

            if (dataflowBlocks != null)
            {
                Complete();
                Completion.GetAwaiter().GetResult();
            }
        }
    }
}
