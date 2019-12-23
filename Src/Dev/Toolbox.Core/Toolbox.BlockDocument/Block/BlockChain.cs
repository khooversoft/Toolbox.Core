// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Security;
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
        private readonly List<BlockNode> _blocks;
        private readonly object _lock = new object();
        private CurrentDigest? _currentDigest = null;

        public BlockChain()
        {
            _blocks = new List<BlockNode>
            {
                new BlockNode(new DataBlock<HeaderBlock>("genesis", "0", new HeaderBlock("genesis"))),
            };
        }

        public BlockChain(IEnumerable<BlockNode> blockNodes)
        {
            blockNodes.Verify(nameof(blockNodes)).IsNotNull();

            _blocks = blockNodes.ToList();
        }

        public IReadOnlyList<BlockNode> Blocks => _blocks;

        public string Digest
        {
            get
            {
                _currentDigest = _currentDigest ?? new CurrentDigest(GetDigest(), _blocks.Count);

                if (((CurrentDigest)_currentDigest).BlockCount != _blocks.Count)
                {
                    _currentDigest = new CurrentDigest(GetDigest(), _blocks.Count);
                }

                return ((CurrentDigest)_currentDigest).Digest;
            }
        }

        public void Add(params IDataBlock[] dataBlocks)
        {
            lock (_lock)
            {
                foreach (var item in dataBlocks)
                {
                    item.Verify().Assert(x => x.GetType() != typeof(BlockNode), "BlockNode is an invalid data block");

                    BlockNode latestBlock = Blocks[_blocks.Count - 1];

                    var newBlock = new BlockNode(item, latestBlock.Index + 1, latestBlock.Digest);
                    _blocks.Add(newBlock);
                }
            }
        }

        public bool IsValid()
        {
            lock (_lock)
            {
                if (Blocks.Any(x => !x.IsValid())) return false;

                for (int i = 1; i < Blocks.Count; i++)
                {
                    BlockNode currentBlock = Blocks[i];
                    BlockNode previousBlock = Blocks[i - 1];

                    if (currentBlock.Digest != currentBlock.Digest) return false;
                    if (currentBlock.PreviousHash != previousBlock.Digest) return false;
                }

                return true;
            }
        }

        public string GetDigest()
        {
            lock (_lock)
            {
                return new MerkleTree()
                .Append(_blocks.Select(x => new MerkleHash(x.GetDigest())).ToArray())
                .BuildTree().ToString();
            }
        }

        public IEnumerator<BlockNode> GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        public static BlockChain operator +(BlockChain self, IDataBlock blockData)
        {
            self.Add(blockData);
            return self;
        }

        private struct CurrentDigest
        {
            public CurrentDigest(string digest, int blockCount)
            {
                Digest = digest;
                BlockCount = blockCount;
            }

            public string Digest { get; }

            public int BlockCount { get; }
        }
    }
}
