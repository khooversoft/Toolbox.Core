// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.BlockDocument;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.BlockDocument.Test.BlobStore
{
    public class BlobContainerTests : IClassFixture<ApplicationFixture>
    {
        private const string _connectionString = "DefaultEndpointsProtocol=https;AccountName=messagehubteststore;AccountKey={blob-storage-test-AccountKey};EndpointSuffix=core.windows.net";
        private readonly BlobStoreConnection _blobStore;
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private readonly ApplicationFixture _application;

        public BlobContainerTests(ApplicationFixture application)
        {
            _application = application;

            string? connectionString = _connectionString.Resolve(_application.PropertyResolver);

            _blobStore = new BlobStoreConnection("blob-storage-test", connectionString!);
        }

        [Fact]
        public async Task GivenBlockChain_WhenContainerIsBlob_ShouldRoundTrip()
        {
            const string _zipPath = "$block";
            const string _blobPath = "Test.sa";

            var container = new BlobRepository(_blobStore);

            await container.CreateContainer(_workContext);

            var blockChain = new BlockChain()
            {
                new DataBlock<HeaderBlock>("header", "header_1", new HeaderBlock("Master Contract")),
                new DataBlock<BlockBlob>("contract", "contract_1", new BlockBlob("contract.docx", "docx", "me", Encoding.UTF8.GetBytes("this is a contract between two people"))),
                new DataBlock<TrxBlock>("ContractLedger", "Pmt", new TrxBlock("1", "cr", 100)),
            };

            blockChain.Chain.Count.Should().Be(4);
            blockChain.IsValid().Should().BeTrue();
            string blockChainHash = blockChain.ToMerkleTree().BuildTree().ToString();

            string json = blockChain.ToJson();

            //var buffer = new byte[1000];
            using var writeMemoryBuffer = new MemoryStream();
            var writer = new ZipContainerWriter(new ZipArchive(writeMemoryBuffer, ZipArchiveMode.Create, leaveOpen: true));
            writer.Write(_workContext, _zipPath, json);
            writer.Close();

            writeMemoryBuffer.Length.Should().BeGreaterThan(0);
            writeMemoryBuffer.Seek(0, SeekOrigin.Begin);

            await container.Delete(_workContext, _blobPath);
            await container.Upload(_workContext, _blobPath, writeMemoryBuffer);
            writeMemoryBuffer.Close();

            IReadOnlyList<byte> readBlob = await container.Download(_workContext, _blobPath);
            using var readMemoryBuffer = new MemoryStream(readBlob.ToArray());

            var reader = new ZipContainerReader(new ZipArchive(readMemoryBuffer, ZipArchiveMode.Read));
            string readJson = reader.Read(_workContext, _zipPath);
            reader.Close();

            BlockChain result = readJson.ToBlockChain();
            blockChain.IsValid().Should().BeTrue();
            string resultChainHash = result.ToMerkleTree().BuildTree().ToString();

            blockChainHash.Should().Be(resultChainHash);

            await container.Delete(_workContext, _blobPath);
        }
    }
}
