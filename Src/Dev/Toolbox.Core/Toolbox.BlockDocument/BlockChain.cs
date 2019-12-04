using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.BlockDocument
{
    public class Blockchain : IEnumerable<Block>
    {
        private readonly List<Block> _chain;

        public Blockchain()
        {
            _chain = new List<Block>
            {
                new Block(0, DateTime.Now, null, "block", "begin", "{}")
            };
        }

        public IReadOnlyList<Block> Chain => _chain;

        public void Add(BlockData dataBlock)
        {
            Block latestBlock = Chain[_chain.Count - 1];

            Block newBlock = new Block(dataBlock, latestBlock.Index + 1, latestBlock.Hash);
            _chain.Add(newBlock);
        }

        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash()) return false;
                if (currentBlock.PreviousHash != previousBlock.Hash) return false;
            }

            return true;
        }

        public IEnumerator<Block> GetEnumerator()
        {
            return _chain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _chain.GetEnumerator();
        }
    }
}
