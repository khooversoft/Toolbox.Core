// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class BlockChain : IEnumerable<BlockNode>
    {
        private readonly List<BlockNode> _chain;
        private readonly object _lock = new object();

        public BlockChain()
        {
            _chain = new List<BlockNode>
            {
                new BlockNode(new DataBlock<HeaderBlock>("genesis", "0", new HeaderBlock("genesis"))),
            };
        }

        public BlockChain(IEnumerable<BlockNode> blockNodes)
        {
            blockNodes.Verify(nameof(blockNodes)).IsNotNull();

            _chain = blockNodes.ToList();
        }

        public IReadOnlyList<BlockNode> Chain => _chain;

        public void Add(params IDataBlock[] dataBlocks)
        {
            lock (_lock)
            {
                foreach (var item in dataBlocks)
                {
                    item.Verify().Assert(x => x.GetType() != typeof(BlockNode), "BlockNode is an invalid data block");

                    BlockNode latestBlock = Chain[_chain.Count - 1];

                    var newBlock = new BlockNode(item, latestBlock.Index + 1, latestBlock.Hash);
                    _chain.Add(newBlock);
                }
            }
        }

        public bool IsValid()
        {
            lock (_lock)
            {
                if (Chain.Any(x => !x.IsValid())) return false;

                for (int i = 1; i < Chain.Count; i++)
                {
                    BlockNode currentBlock = Chain[i];
                    BlockNode previousBlock = Chain[i - 1];

                    if (currentBlock.Hash != currentBlock.Hash) return false;
                    if (currentBlock.PreviousHash != previousBlock.Hash) return false;
                }

                return true;
            }
        }

        public IEnumerator<BlockNode> GetEnumerator()
        {
            return _chain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _chain.GetEnumerator();
        }

        public static BlockChain operator +(BlockChain self, IDataBlock blockData)
        {
            self.Add(blockData);
            return self;
        }
    }
}
