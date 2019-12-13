using FluentAssertions;
using Khooversoft.Toolbox.BlockDocument;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Toolbox.BlockDocument.Test
{
    public class JsonSerializationTests
    {
        [Fact]
        public void GivenBlockChain_WhenSerialized_ShouldPass()
        {
            var block1 = new DataBlock<string>(DateTime.Now, "blockTypeV1", "blockIdV1", "dataV1");

            string json = JsonConvert.SerializeObject(block1.ConvertTo());
            json.Should().NotBeNullOrWhiteSpace();

            DataBlockModel<string> resultModel = JsonConvert.DeserializeObject<DataBlockModel<string>>(json);
            resultModel.Should().NotBeNull();

            DataBlock<string> result = resultModel.ConvertTo();
            result.Should().NotBeNull();

            block1.TimeStamp.Should().Be(result.TimeStamp);
            block1.BlockType.Should().Be(result.BlockType);
            block1.BlockId.Should().Be(result.BlockId);
            block1.Data.Should().Be(result.Data);
        }
    }
}
