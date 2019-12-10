using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Toolbox.BlockDocument.Test
{
    public class BlockChainTests
    {
        [Fact]
        public void GivenBlockChain_AppendSingleNode_ShouldVerify()
        {
            var now = DateTime.Now;
            var blockChain = new BlockChain();

            var block1 = new BlockNode(new BlockData<string>(now, "blockTypeV1", "blockIdV1", "dataV1"), 1,"hashV1");
            blockChain.Add(block1);

            blockChain.IsValid().Should().BeTrue();
        }

        [Fact]
        public void GivenBlockChain_AppendTwoNode_ShouldVerify()
        {
            var now = DateTime.Now;
            var blockChain = new BlockChain();

            var block1 = new BlockNode(new BlockData<string>(now, "blockTypeV1", "blockIdV1", "dataV1"), 1, "hashV1");
            blockChain.Add(block1);

            var block2 = new BlockNode(new BlockData<string>(now, "blockTypeV2", "blockIdV2", "dataV2"), 1, block1.Hash);
            blockChain.Add(block2);

            block1.Hash.Should().Be(block2.PreviousHash);

            blockChain.IsValid().Should().BeTrue();
        }

        [Fact]
        public void GivenBlockChain_AppendManyNode_ShouldVerify()
        {
            var now = DateTime.Now;
            const int max = 10;
            var blockChain = new BlockChain();

            List<BlockNode> list = Enumerable.Range(0, max)
                .Select(x => new BlockNode(new BlockData<string>(now, $"blockTypeV{x}", $"blockIdV{x}", $"dataV{x}"), 1, $"hashV{x}"))
                .ToList();

            blockChain.Add(list.ToArray());

            blockChain.IsValid().Should().BeTrue();
        }
    }
}
