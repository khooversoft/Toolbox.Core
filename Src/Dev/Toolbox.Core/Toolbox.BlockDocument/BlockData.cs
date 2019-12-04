using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Toolbox.BlockDocument
{
    public class BlockData
    {
        public BlockData(DateTime timeStamp, string blockType, string blockId, string data)
        {
            blockType.Verify(nameof(blockType)).IsNotEmpty();
            blockId.Verify(nameof(blockId)).IsNotEmpty();
            data.Verify(nameof(data)).IsNotEmpty();

            TimeStamp = timeStamp;
            BlockType = blockType;
            BlockId = blockId;
            Data = data;
        }

        public DateTime TimeStamp { get; }

        public string BlockType { get; }

        public string BlockId { get; }

        public string Data { get; }
    }
}
