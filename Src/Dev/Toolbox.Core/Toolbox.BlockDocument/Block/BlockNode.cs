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
    public class BlockNode
    {
        public BlockNode(IDataBlock blockData)
        {
            blockData.Verify(nameof(blockData)).IsNotNull();

            BlockData = blockData;
            Hash = GetUTF8Bytes().ComputeSha256Hash();
        }

        public BlockNode(IDataBlock blockData, int index, string? previousHash)
        {
            blockData.Verify(nameof(blockData)).IsNotNull();

            BlockData = blockData;
            Index = index;
            PreviousHash = previousHash;
            Hash = GetUTF8Bytes().ComputeSha256Hash();
        }

        public BlockNode(BlockNode blockNode)
            : this(blockNode.BlockData, blockNode.Index, blockNode.PreviousHash!)
        {
        }

        public IDataBlock BlockData { get; }

        public int Index { get; }

        public string? PreviousHash { get; }

        public string Hash { get; }

        public bool IsValid()
        {
            return Hash == GetUTF8Bytes().ComputeSha256Hash();
        }

        public IReadOnlyList<byte> GetUTF8Bytes()
        {
            return BlockData.GetBytesForHash()
                .Concat(Encoding.UTF8.GetBytes($"{Index}-{PreviousHash ?? ""}"))
                .ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj is BlockNode blockNode)
            {
                return Index == blockNode.Index &&
                    PreviousHash == blockNode.PreviousHash &&
                    Hash == blockNode.Hash;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode() ^
                PreviousHash?.GetHashCode() ?? 0 ^
                Hash.GetHashCode();
        }

        public static bool operator ==(BlockNode v1, BlockNode v2) => v1.Equals(v2);

        public static bool operator !=(BlockNode v1, BlockNode v2) => !v1.Equals(v2);
    }
}
