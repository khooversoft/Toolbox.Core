// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Azure;
using FluentAssertions;
using Khoover.Toolbox.TestTools;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ToolBox.Azure.Test.Blob
{
    [Collection("QueueTests")]
    public class BlobRegistorStoreTests
    {
        private readonly ILoggerFactory _testLoggerFactory = new TestLoggerFactory();
        private readonly AzureTestOption _testOption;

        public BlobRegistorStoreTests()
        {
            _testOption = new TestOptionBuilder().Build();
        }

        [Trait("Category", "LocalOnly")]
        [Fact]
        public async Task WhenContainer_CreateIfItDoesNotExist_ShouldPass()
        {
            var container = _testOption.GetBlobRepository(_testLoggerFactory);

            await container.CreateContainer(CancellationToken.None);
        }

        [Trait("Category", "LocalOnly")]
        [Fact]
        public async Task GivenFileDoesNotExist_WhenGet_ShouldThrow()
        {
            var container = _testOption.GetBlobRepository(_testLoggerFactory);

            await container.CreateContainer(CancellationToken.None);

            IReadOnlyList<string> filePaths = await container.List("*");
            await filePaths.ForEachAsync(async x => await container.Delete(x, CancellationToken.None));

            const string filePath = "testBlob";
            Func<Task> act = async () => await container.Get(filePath);

            await act.Should().ThrowAsync<RequestFailedException>();
        }

        [Trait("Category", "LocalOnly")]
        [Fact]
        public async Task WhenContainer_FullLiveCycle_ShouldPass()
        {
            var container = _testOption.GetBlobRepository(_testLoggerFactory);

            await container.CreateContainer(CancellationToken.None);

            IReadOnlyList<string> filePaths = await container.List("*");
            await filePaths.ForEachAsync(async x => await container.Delete(x, CancellationToken.None));

            const string filePath = "testBlob";
            const string data = "This is data";
            await container.Set(filePath, data, CancellationToken.None);

            filePaths = await container.List("*");
            filePaths.Should().NotBeNull();
            filePaths.Count.Should().Be(1);
            filePaths[0].Should().Be(filePath);

            string readData = await container.Get(filePath);
            readData.Should().NotBeNullOrWhiteSpace();
            readData.Should().Be(data);

            await container.Delete(filePath, CancellationToken.None);

            filePaths = await container.List("*");
            filePaths.Should().NotBeNull();
            filePaths.Count.Should().Be(0);
        }

        [Trait("Category", "LocalOnly")]
        [Fact]
        public async Task WhenContainer_FullLiveCycleForFiles_ShouldPass()
        {
            var container = _testOption.GetBlobRepository(_testLoggerFactory);

            await container.CreateContainer(CancellationToken.None);

            IReadOnlyList<string> filePaths = await container.List("*");
            await filePaths.ForEachAsync(x => container.Delete(x, CancellationToken.None));

            const int count = 10;
            var dataList = Enumerable.Range(0, count)
                .Select(x => new { File = $"File_{x}", Data = $"Data_{x}" })
                .ToList();

            await dataList.ForEachAsync(x => container.Set(x.File, x.Data, CancellationToken.None));

            filePaths = await container.List("*");
            filePaths.Should().NotBeNull();
            filePaths.Count.Should().Be(count);

            await dataList.ForEachAsync(x => container.Delete(x.File, CancellationToken.None));

            filePaths = await container.List("*");
            filePaths.Should().NotBeNull();
            filePaths.Count.Should().Be(0);
        }
    }
}
