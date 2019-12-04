using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Toolbox.BlockDocument
{
    public class Block : BlockData
    {
        public Block(BlockData blockData, int index, string previousHash)
            : base(blockData.TimeStamp, blockData.BlockType, blockData.BlockId, blockData.Data)
        {
            previousHash.Verify(nameof(previousHash)).IsNotEmpty();

            Index = index;
            PreviousHash = previousHash;
            Hash = CalculateHash();
        }
        
        public Block(int index, DateTime timeStamp, string previousHash, string blockType, string blockId, string data)
            : base(timeStamp, blockType, blockId, data)
        {
            previousHash.Verify(nameof(previousHash)).IsNotEmpty();

            Index = index;
            PreviousHash = previousHash;
            Hash = CalculateHash();
        }

        public int Index { get; }

        public string PreviousHash { get; }

        public string Hash { get; }

        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes($"{Index}-{TimeStamp}-{PreviousHash ?? ""}-{BlockType}-{BlockId}-{Data}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }
    }
}
