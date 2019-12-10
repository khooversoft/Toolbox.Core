using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Toolbox.BlockDocument
{
    public class BlockData<T> : IBlockData
    {
        public BlockData(DateTime timeStamp, string blockType, string blockId, T data)
        {
            blockType.Verify(nameof(blockType)).IsNotEmpty();
            blockId.Verify(nameof(blockId)).IsNotEmpty();
            data.Verify(nameof(data)).IsNotNull();

            TimeStamp = timeStamp;
            BlockType = blockType;
            BlockId = blockId;
            Data = data;
        }

        public BlockData(BlockData<T> blockData)
        {
            blockData.Verify(nameof(blockData)).IsNotNull();

            TimeStamp = blockData.TimeStamp;
            BlockType = blockData.BlockType;
            BlockId = blockData.BlockId;
            Data = blockData.Data;
        }

        public DateTime TimeStamp { get; }

        public string BlockType { get; }

        public string BlockId { get; }

        public T Data { get; }

        public byte[] GetUTF8Bytes()
        {
            byte[] append;

            switch(Data)
            {
                case string str:
                    append = Encoding.UTF8.GetBytes(str);
                    break;

                case IBlockData dataBlock:
                    append = dataBlock.GetUTF8Bytes();
                    break;

                default:
                    throw new InvalidOperationException($"Data {Data.GetType().Name} is not a string or implements IBlockData");
            }

            return Encoding.UTF8.GetBytes($"{TimeStamp}-{BlockType}-{BlockId}-")
                .Concat(append)
                .ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj is BlockData<T> blockData)
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

        public static bool operator ==(BlockData<T> v1, BlockData<T> v2) => v1.Equals(v2);

        public static bool operator !=(BlockData<T> v1, BlockData<T> v2) => !v1.Equals(v2);
    }
}
