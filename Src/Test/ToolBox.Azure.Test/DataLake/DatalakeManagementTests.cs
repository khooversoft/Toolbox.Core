using FluentAssertions;
using Khoover.Toolbox.TestTools;
using Khooversoft.Toolbox.Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ToolBox.Azure.Test.DataLake
{
    [Collection("DatalakeTests")]
    public class DatalakeManagementTests
    {
        private readonly AzureTestOption _testOption;
        private readonly ILoggerFactory _loggerFactory = new TestLoggerFactory();

        public DatalakeManagementTests()
        {
            _testOption = new TestOptionBuilder().Build();
        }

        [Trait("Category", "Unit")]
        [Fact]
        public async Task GivenFileSystem_WhenExist_SearchDoesReturn()
        {
            string fileSystemName = _testOption.DatalakeOption.FileSystemName + "1";

            IDatalakeManagement management = _testOption.GetDatalakeManagement(_loggerFactory);
            await management.DeleteIfExist(fileSystemName, CancellationToken.None);

            await management.Create(fileSystemName, CancellationToken.None);

            IReadOnlyList<string> list = await management.List(CancellationToken.None);
            list.Count(x => x == fileSystemName).Should().Be(1);

            await management.Delete(fileSystemName, CancellationToken.None);
        }

        [Trait("Category", "Unit")]
        [Fact]
        public async Task GivenFileSystem_WhenNotExist_SearchDoesNotReturn()
        {
            string fileSystemName = _testOption.DatalakeOption.FileSystemName + "2";

            IDatalakeManagement management = _testOption.GetDatalakeManagement(_loggerFactory);
            await management.DeleteIfExist(fileSystemName, CancellationToken.None);

            IReadOnlyList<string> list = await management.List(CancellationToken.None);
            list.Count(x => x == fileSystemName).Should().Be(0);
        }

        [Trait("Category", "Unit")]
        [Fact]
        public async Task GivenFileSystem_WhenNotExist_CreateShouldExist()
        {
            string fileSystemName = _testOption.DatalakeOption.FileSystemName + "3";

            IDatalakeManagement management = _testOption.GetDatalakeManagement(_loggerFactory);
            await management.DeleteIfExist(fileSystemName, CancellationToken.None);

            await management.CreateIfNotExist(fileSystemName, CancellationToken.None);

            IReadOnlyList<string> list = await management.List(CancellationToken.None);
            list.Count(x => x == fileSystemName).Should().Be(1);

            await management.Delete(fileSystemName, CancellationToken.None);
        }
    }
}
