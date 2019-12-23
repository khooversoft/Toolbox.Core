using Khooversoft.Toolbox.Standard;
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

        public static Stream ToZipContainer(this BlockChain blockChain, IWorkContext context)
        {
            blockChain.Verify(nameof(blockChain)).IsNotNull();
            blockChain.IsValid().Verify().Assert(x => x == true, "Block chain is not valid");

            var writeMemoryBuffer = new MemoryStream();
            using var writer = new ZipContainerWriter(new ZipArchive(writeMemoryBuffer, ZipArchiveMode.Create, leaveOpen: true));

            try
            {
                string blockChainHash = blockChain.ToMerkleTree().BuildTree().ToString();
                writer.Write(context, _merkleTreeHash, blockChainHash);

                string json = blockChain.ToJson();
                writer.Write(context, _zipPath, json);
            }
            finally
            {
                writer.Close();
            }

            writeMemoryBuffer.Length.Verify().Assert(x => x > 0, "ZipContainer memory buffer length is zero");
            writeMemoryBuffer.Seek(0, SeekOrigin.Begin);

            return writeMemoryBuffer;
        }

        public static BlockChain ToBlockChain(this Stream readFromStream, IWorkContext context)
        {
            readFromStream.Verify(nameof(readFromStream)).IsNotNull();

            using var reader = new ZipContainerReader(new ZipArchive(readFromStream, ZipArchiveMode.Read));

            string readJson = reader.Read(context, _zipPath);
            string blockChainHash = reader.Read(context, _merkleTreeHash);

            BlockChain result = readJson.ToBlockChain();
            result.IsValid().Verify().Assert(x => x == true, "Read block chain is invalid");
            string resultChainHash = result.ToMerkleTree().BuildTree().ToString();

            (blockChainHash == resultChainHash).Verify().Assert(x => x == true, "Merkle hash does not match, block chain is invalid");
            return result;
        }
    }
}
