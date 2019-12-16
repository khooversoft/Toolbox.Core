// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Azure;
using FluentAssertions;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ToolBox.Azure.Test.Blob
{
    [Collection("QueueTests")]
    public class BlobRegistorStoreTests : IClassFixture<ApplicationFixture>
    {
        private const string _connectionString = "DefaultEndpointsProtocol=https;AccountName=messagehubteststore;AccountKey={blob-storage-test-AccountKey};EndpointSuffix=core.windows.net";
        private readonly BlobStoreConnection _blobStore;
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private readonly ApplicationFixture _application;

        public BlobRegistorStoreTests(ApplicationFixture application)
        {
            _application = application;

            string connectionString = _connectionString.Resolve(_application.PropertyResolver);

            _blobStore = new BlobStoreConnection("blob-storage-test", connectionString);
        }

        [Fact]
        public async Task WhenContainer_CreateIfItDoesNotExist_ShouldPass()
        {
            var subject = new BlobRepository(_blobStore);

            await subject.CreateContainer(_workContext);
        }

        [Fact]
        public async Task GivenFileDoesNotExist_WhenGet_ShouldThrow()
        {
            var container = new BlobRepository(_blobStore);

            await container.CreateContainer(_workContext);

            IReadOnlyList<string> filePaths = await container.List(_workContext, "*");
            await filePaths.ForEachAsync(x => container.Delete(_workContext, x));

            const string filePath = "testBlob";
            Func<Task> act = async () => await container.Get(_workContext, filePath);

            await act.Should().ThrowAsync<RequestFailedException>();
        }

        [Fact]
        public async Task WhenContainer_FullLiveCycle_ShouldPass()
        {
            var container = new BlobRepository(_blobStore);

            await container.CreateContainer(_workContext);

            IReadOnlyList<string> filePaths = await container.List(_workContext, "*");
            await filePaths.ForEachAsync(x => container.Delete(_workContext, x));

            const string filePath = "testBlob";
            const string data = "This is data";
            await container.Set(_workContext, filePath, data);

            filePaths = await container.List(_workContext, "*");
            filePaths.Should().NotBeNull();
            filePaths.Count.Should().Be(1);
            filePaths[0].Should().Be(filePath);

            string readData = await container.Get(_workContext, filePath);
            readData.Should().NotBeNullOrWhiteSpace();
            readData.Should().Be(data);

            await container.Delete(_workContext, filePath);

            filePaths = await container.List(_workContext, "*");
            filePaths.Should().NotBeNull();
            filePaths.Count.Should().Be(0);
        }

        [Fact]
        public async Task WhenContainer_FullLiveCycleForFiles_ShouldPass()
        {
            var container = new BlobRepository(_blobStore);

            await container.CreateContainer(_workContext);

            IReadOnlyList<string> filePaths = await container.List(_workContext, "*");
            await filePaths.ForEachAsync(x => container.Delete(_workContext, x));

            const int count = 10;
            var dataList = Enumerable.Range(0, count)
                .Select(x => new { File = $"File_{x}", Data = $"Data_{x}" })
                .ToList();

            await dataList.ForEachAsync(x => container.Set(_workContext, x.File, x.Data));

            filePaths = await container.List(_workContext, "*");
            filePaths.Should().NotBeNull();
            filePaths.Count.Should().Be(count);

            await dataList.ForEachAsync(x => container.Delete(_workContext, x.File));

            filePaths = await container.List(_workContext, "*");
            filePaths.Should().NotBeNull();
            filePaths.Count.Should().Be(0);
        }
    }
}
