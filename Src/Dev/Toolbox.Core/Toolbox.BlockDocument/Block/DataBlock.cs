using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class DataBlock<T> : IBlockData
    {
        public DataBlock(string blockType, string blockId, T data)
        {
            blockType.Verify(nameof(blockType)).IsNotEmpty();
            blockId.Verify(nameof(blockId)).IsNotEmpty();
            data.Verify(nameof(data)).IsNotNull();

            TimeStamp = DateTime.UtcNow;
            BlockType = blockType;
            BlockId = blockId;
            Data = data;
        }

        public DataBlock(DateTime timeStamp, string blockType, string blockId, T data)
        {
            blockType.Verify(nameof(blockType)).IsNotEmpty();
            blockId.Verify(nameof(blockId)).IsNotEmpty();
            data.Verify(nameof(data)).IsNotNull();

            TimeStamp = timeStamp;
            BlockType = blockType;
            BlockId = blockId;
            Data = data;
        }

        public DataBlock(DataBlock<T> dataBlock)
        {
            dataBlock.Verify(nameof(dataBlock)).IsNotNull();

            TimeStamp = dataBlock.TimeStamp;
            BlockType = dataBlock.BlockType;
            BlockId = dataBlock.BlockId;
            Data = dataBlock.Data;
        }

        public DateTime TimeStamp { get; }

        public string BlockType { get; }

        public string BlockId { get; }

        public T Data { get; }

        public IReadOnlyList<byte> GetUTF8Bytes()
        {
            var append = Data switch
            {
                string str => Encoding.UTF8.GetBytes(str),

                IBlockData dataBlock => dataBlock.GetUTF8Bytes(),

                _ => throw new InvalidOperationException($"Data {Data.GetType().Name} is not a string or implements IBlockData"),
            };

            return Encoding.UTF8.GetBytes($"{TimeStamp}-{BlockType}-{BlockId}-")
                .Concat(append)
                .ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj is DataBlock<T> blockData)
            {
                return TimeStamp == blockData.TimeStamp &&
                    BlockType == blockData.BlockType &&
                    BlockId == blockData.BlockId &&
                    Data.Equals(blockData.Data);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return TimeStamp.GetHashCode() ^
                BlockType.GetHashCode() ^
                BlockId.GetHashCode() ^
                Data.GetHashCode();
        }

        public static bool operator ==(DataBlock<T> v1, DataBlock<T> v2) => v1.Equals(v2);

        public static bool operator !=(DataBlock<T> v1, DataBlock<T> v2) => !v1.Equals(v2);
    }
}
