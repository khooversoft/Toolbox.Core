using FluentAssertions;
using Khooversoft.Toolbox.BlockDocument;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Xunit;

namespace Toolbox.BlockDocument.Test
{
    public class ZipContainerTests
    {
        private readonly IWorkContext _workContext = WorkContext.Empty;

        [Fact]
        public void GivenBlockChain_WhenContainerIsMemory_ShouldRoundTrip()
        {
            const string _zipPath = "$block";

            var blockChain = new BlockChain()
            {
                new DataBlock<HeaderBlock>("header", "header_1", new HeaderBlock("Master Contract")),
                new DataBlock<BlobBlock>("contract", "contract_1", new BlobBlock("contract.docx", "docx", "me", Encoding.UTF8.GetBytes("this is a contract between two people"))),
                new DataBlock<TrxBlock>("ContractLedger", "Pmt", new TrxBlock("1", "cr", 100)),
            };

            blockChain.Blocks.Count.Should().Be(4);
            blockChain.IsValid().Should().BeTrue();
            string blockChainHash = blockChain.ToMerkleTree().BuildTree().ToString();

            string json = blockChain.ToJson();

            var buffer = new byte[1000];
            using var memoryBuffer = new MemoryStream(buffer);
            var writer = new ZipContainerWriter(new ZipArchive(memoryBuffer, ZipArchiveMode.Create, leaveOpen: true));
            writer.Write(_workContext, _zipPath, json);
            writer.Close();

            memoryBuffer.Length.Should().BeGreaterThan(0);
            memoryBuffer.Seek(0, SeekOrigin.Begin);

            var reader = new ZipContainerReader(new ZipArchive(memoryBuffer, ZipArchiveMode.Read, leaveOpen: true));
            string readJson = reader.Read(_workContext, _zipPath);
            reader.Close();

            BlockChain result = readJson.ToBlockChain();
            blockChain.IsValid().Should().BeTrue();
            string resultChainHash = result.ToMerkleTree().BuildTree().ToString();

            blockChainHash.Should().Be(resultChainHash);
        }

        [Fact]
        public void GivenBlockChain_WhenContainerIsFile_ShouldRoundTrip()
        {
            const string _zipPath = "$block";

            var blockChain = new BlockChain()
            {
                new DataBlock<HeaderBlock>("header", "header_1", new HeaderBlock("Master Contract")),
                new DataBlock<BlobBlock>("contract", "contract_1", new BlobBlock("contract.docx", "docx", "me", Encoding.UTF8.GetBytes("this is a contract between two people"))),
                new DataBlock<TrxBlock>("ContractLedger", "Pmt", new TrxBlock("1", "cr", 100)),
            };

            blockChain.Blocks.Count.Should().Be(4);
            blockChain.IsValid().Should().BeTrue();
            string blockChainHash = blockChain.ToMerkleTree().BuildTree().ToString();

            string json = blockChain.ToJson();

            string tempFile = Path.GetTempFileName();
            var writer = new ZipContainerWriter(tempFile).OpenFile(_workContext);
            writer.Write(_workContext, _zipPath, json);
            writer.Close();

            var reader = new ZipContainerReader(tempFile).OpenFile(_workContext);
            reader.Exist(_workContext, _zipPath).Should().BeTrue();

            string readJson = reader.Read(_workContext, _zipPath);
            reader.Close();
            File.Delete(tempFile);

            BlockChain result = readJson.ToBlockChain();
            blockChain.IsValid().Should().BeTrue();
            string resultChainHash = result.ToMerkleTree().BuildTree().ToString();

            blockChainHash.Should().Be(resultChainHash);
        }
    }
}
