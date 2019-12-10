using Khooversoft.Toolbox.Standard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.BlockDocument
{
    public class BlockChain : IEnumerable<BlockNode>
    {
        private readonly List<BlockNode> _chain;
        private readonly object _lock = new object();

        public BlockChain()
        {
            _chain = new List<BlockNode>
            {
                new BlockNode(new BlockData<string>(DateTime.Now, "block", "begin", "{}"), 0, "*"),
            };
        }

        public IReadOnlyList<BlockNode> Chain => _chain;

        public void Add(params BlockNode[] dataBlock)
        {
            lock (_lock)
            {
                foreach (var item in dataBlock)
                {
                    BlockNode latestBlock = Chain[_chain.Count - 1];

                    BlockNode newBlock = new BlockNode(item, latestBlock.Index + 1, latestBlock.Hash);
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
    }
}
