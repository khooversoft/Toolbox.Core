// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class DataBlock<T> : IDataBlock
        where T : IBlockType
    {
        public DataBlock(string blockType, string blockId, T data, IEnumerable<KeyValuePair<string, string>>? properties = null)
        {
            blockType.Verify(nameof(blockType)).IsNotEmpty();
            blockId.Verify(nameof(blockId)).IsNotEmpty();
            data.Verify(nameof(data)).IsNotNull();

            TimeStamp = DateTime.UtcNow;
            BlockType = blockType;
            BlockId = blockId;
            Data = data;
            Properties = properties?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, string>();
        }

        public DataBlock(DateTime timeStamp, string blockType, string blockId, T data, IEnumerable<KeyValuePair<string, string>>? properties = null)
            : this(blockType, blockId, data, properties)
        {
            TimeStamp = timeStamp;
        }

        public DataBlock(DataBlock<T> dataBlock)
        {
            dataBlock.Verify(nameof(dataBlock)).IsNotNull();

            TimeStamp = dataBlock.TimeStamp;
            BlockType = dataBlock.BlockType;
            BlockId = dataBlock.BlockId;
            Data = dataBlock.Data;
            Properties = dataBlock.Properties.ToDictionary(x => x.Key, x => x.Value);
        }

        public DateTime TimeStamp { get; }

        public string BlockType { get; }

        public string BlockId { get; }

        public T Data { get; }

        public IReadOnlyDictionary<string, string> Properties { get; set; }

        public IReadOnlyList<byte> GetBytesForHash()
        {
            return Encoding.UTF8.GetBytes($"{TimeStamp}-{BlockType}-{BlockId}-")
                .Concat(Data.GetBytesForHash())
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
