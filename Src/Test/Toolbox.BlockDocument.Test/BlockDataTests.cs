using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Khooversoft.Toolbox.Standard;
using Khooversoft.Toolbox.BlockDocument;

namespace Toolbox.BlockDocument.Test
{
    public class BlockDataTests
    {
        [Fact]
        public void GivenBlockData_WhenValuesSet_VerifyNoChange()
        {
            var now = DateTime.Now;
            var data = new DataBlock<string>(now, "blockType", "blockId", "data");

            data.TimeStamp.Should().Be(now);
            data.BlockType.Should().Be("blockType");
            data.BlockId.Should().Be("blockId");
            data.Data.Should().Be("data");
        }

        [Fact]
        public void GivenBlockData_WhenTestForEqual_ShouldPass()
        {
            var now = DateTime.Now;
            var data = new DataBlock<string>(now, "blockType", "blockId", "data");

            var v2 = data;
            (data == v2).Should().BeTrue();
            (data != v2).Should().BeFalse();

            data.TimeStamp.Should().Be(v2.TimeStamp);
            data.BlockType.Should().Be(v2.BlockType);
            data.BlockId.Should().Be(v2.BlockId);
            data.Data.Should().Be(v2.Data);

            var v3 = new DataBlock<string>(data);
            (data == v3).Should().BeTrue();
            (data != v3).Should().BeFalse();

            data.TimeStamp.Should().Be(v3.TimeStamp);
            data.BlockType.Should().Be(v3.BlockType);
            data.BlockId.Should().Be(v3.BlockId);
            data.Data.Should().Be(v3.Data);
        }

        [Fact]
        public void GivenBlockNode_WhenValueSet_VerifyNoChange()
        {
            var now = DateTime.Now;
            var data = new BlockNode(new DataBlock<string>(now, "blockType", "blockId", "datav2"), 1, "previousHash");
            data.IsValid().Should().BeTrue();

            data.Index.Should().Be(1);
            data.PreviousHash.Should().Be("previousHash");
            data.BlockData.As<DataBlock<string>>().TimeStamp.Should().Be(now);
            data.BlockData.As<DataBlock<string>>().BlockType.Should().Be("blockType");
            data.BlockData.As<DataBlock<string>>().BlockId.Should().Be("blockId");
            data.BlockData.As<DataBlock<string>>().Data.Should().Be("datav2");
        }

        [Fact]
        public void GivenBlockNode_WhenTestForEqual_VerifyNoChange()
        {
            var now = DateTime.Now;
            var data = new BlockNode(new DataBlock<string>(now, "blockType", "blockId", "datav2"), 1, "previousHash");
            data.IsValid().Should().BeTrue();

            var v2 = data;
            v2.IsValid().Should().BeTrue();
            (data == v2).Should().BeTrue();
            (data != v2).Should().BeFalse();

            data.Index.Should().Be(v2.Index);
            data.PreviousHash.Should().Be(v2.PreviousHash);
            data.BlockData.As<DataBlock<string>>().TimeStamp.Should().Be(now);
            data.BlockData.As<DataBlock<string>>().BlockType.Should().Be(v2.BlockData.As<DataBlock<string>>().BlockType);
            data.BlockData.As<DataBlock<string>>().BlockId.Should().Be(v2.BlockData.As<DataBlock<string>>().BlockId);
            data.BlockData.As<DataBlock<string>>().Data.Should().Be(v2.BlockData.As<DataBlock<string>>().Data);

            var v3 = new BlockNode(data);
            v3.IsValid().Should().BeTrue();
            (data == v3).Should().BeTrue();
            (data != v3).Should().BeFalse();

            data.Index.Should().Be(v3.Index);
            data.PreviousHash.Should().Be(v3.PreviousHash);
            data.BlockData.As<DataBlock<string>>().TimeStamp.Should().Be(now);
            data.BlockData.As<DataBlock<string>>().BlockType.Should().Be(v3.BlockData.As<DataBlock<string>>().BlockType);
            data.BlockData.As<DataBlock<string>>().BlockId.Should().Be(v3.BlockData.As<DataBlock<string>>().BlockId);
            data.BlockData.As<DataBlock<string>>().Data.Should().Be(v3.BlockData.As<DataBlock<string>>().Data);
        }
    }
}
