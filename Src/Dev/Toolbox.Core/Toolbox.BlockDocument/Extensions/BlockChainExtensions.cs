// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public static class BlockChainExtensions
    {
        private const string _zipPath = "$BlockChain";
        private const string _merkleTreeHash = "$MerkleTreeHash";

        public static Stream ToZipContainer(this BlockChain blockChain)
        {
            blockChain.VerifyNotNull(nameof(blockChain));
            blockChain.IsValid().VerifyAssert(x => x == true, "Block chain is not valid");

            var writeMemoryBuffer = new MemoryStream();
            using var writer = new ZipContainerWriter(new ZipArchive(writeMemoryBuffer, ZipArchiveMode.Create, leaveOpen: true));

            try
            {
                string blockChainHash = blockChain.ToMerkleTree().BuildTree().ToString();
                writer.Write(_merkleTreeHash, blockChainHash);

                string json = blockChain.ToJson();
                writer.Write(_zipPath, json);
            }
            finally
            {
                writer.Close();
            }

            writeMemoryBuffer.Length.VerifyAssert(x => x > 0, "ZipContainer memory buffer length is zero");
            writeMemoryBuffer.Seek(0, SeekOrigin.Begin);

            return writeMemoryBuffer;
        }

        public static BlockChain ToBlockChain(this Stream readFromStream)
        {
            readFromStream.VerifyNotNull(nameof(readFromStream));

            using var reader = new ZipContainerReader(new ZipArchive(readFromStream, ZipArchiveMode.Read));

            string readJson = reader.Read(_zipPath);
            string blockChainHash = reader.Read(_merkleTreeHash);

            BlockChain result = readJson.ToBlockChain();
            result.IsValid().VerifyAssert(x => x == true, "Read block chain is invalid");
            string resultChainHash = result.ToMerkleTree().BuildTree().ToString();

            (blockChainHash == resultChainHash).VerifyAssert(x => x == true, "Merkle hash does not match, block chain is invalid");
            return result;
        }
    }
}
