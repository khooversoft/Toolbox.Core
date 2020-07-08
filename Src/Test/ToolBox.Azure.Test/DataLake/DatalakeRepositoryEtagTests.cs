using FluentAssertions;
using Khoover.Toolbox.TestTools;
using Khooversoft.Toolbox.Azure;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ToolBox.Azure.Test.DataLake
{
    [Collection("DatalakeTests")]
    public class DatalakeRepositoryEtagTests
    {
        private readonly AzureTestOption _testOption;
        private readonly ILoggerFactory _loggerFactory = new TestLoggerFactory();

        public DatalakeRepositoryEtagTests()
        {
            _testOption = new TestOptionBuilder().Build();
        }

        [Trait("Category", "Unit")]
        [Fact]
        public async Task GivenData_WhenSaved_ShouldMatchEtag()
        {
            const string data = "this is a test";
            const string path = "testStringEtag.txt";

            IDatalakeRepository datalakeRepository = _testOption.GetDatalakeRepository(_loggerFactory);

            await _testOption
                .GetDatalakeManagement(_loggerFactory)
                .CreateIfNotExist(_testOption.DatalakeOption.FileSystemName, CancellationToken.None);

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            await datalakeRepository.Write(path, dataBytes, true, CancellationToken.None);

            byte[] receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();
            Enumerable.SequenceEqual(dataBytes, receive).Should().BeTrue();

            DatalakePathProperties? pathProperties = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties.Should().NotBeNull();
            pathProperties!.ETag.Should().NotBeNullOrEmpty();

            receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();

            DatalakePathProperties? pathProperties2ndRead = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties2ndRead.Should().NotBeNull();
            pathProperties2ndRead!.ETag.Should().NotBeNullOrEmpty();

            pathProperties.ETag.Should().Be(pathProperties2ndRead.ETag);

            await datalakeRepository.Delete(path, CancellationToken.None);
        }

        [Trait("Category", "Unit")]
        [Fact]
        public async Task GivenData_WhenSavedAndWritten_ShouldNotMatchEtag()
        {
            const string data = "this is a test";
            const string path = "testStringEtag.txt";

            IDatalakeRepository datalakeRepository = _testOption.GetDatalakeRepository(_loggerFactory);

            await _testOption
                .GetDatalakeManagement(_loggerFactory)
                .CreateIfNotExist(_testOption.DatalakeOption.FileSystemName, CancellationToken.None);

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            await datalakeRepository.Write(path, dataBytes, true, CancellationToken.None);

            byte[] receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();
            Enumerable.SequenceEqual(dataBytes, receive).Should().BeTrue();

            DatalakePathProperties? pathProperties = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties.Should().NotBeNull();
            pathProperties!.ETag.Should().NotBeNullOrEmpty();

            await datalakeRepository.Write(path, dataBytes, true, CancellationToken.None);
            receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();

            DatalakePathProperties? pathProperties2ndRead = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties2ndRead.Should().NotBeNull();
            pathProperties2ndRead!.ETag.Should().NotBeNullOrEmpty();

            pathProperties.ETag.Should().NotBe(pathProperties2ndRead.ETag);

            await datalakeRepository.Delete(path, CancellationToken.None);
        }

        [Trait("Category", "Unit")]
        [Fact]
        public async Task GivenData_WhenSavedAndUpdate_ShouldNotMatchEtag()
        {
            const string data = "this is a test";
            const string data2 = "this is a test2";
            const string path = "testStringEtag.txt";

            IDatalakeRepository datalakeRepository = _testOption.GetDatalakeRepository(_loggerFactory);

            await _testOption
                .GetDatalakeManagement(_loggerFactory)
                .CreateIfNotExist(_testOption.DatalakeOption.FileSystemName, CancellationToken.None);

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            await datalakeRepository.Write(path, dataBytes, true, CancellationToken.None);

            byte[] receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();
            Enumerable.SequenceEqual(dataBytes, receive).Should().BeTrue();

            DatalakePathProperties? pathProperties = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties.Should().NotBeNull();
            pathProperties!.ETag.Should().NotBeNullOrEmpty();

            byte[] data2Bytes = Encoding.UTF8.GetBytes(data2);
            await datalakeRepository.Write(path, data2Bytes, true, CancellationToken.None);

            receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();

            DatalakePathProperties? pathProperties2ndRead = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties2ndRead.Should().NotBeNull();
            pathProperties2ndRead!.ETag.Should().NotBeNullOrEmpty();

            pathProperties.ETag.Should().NotBe(pathProperties2ndRead.ETag);

            await datalakeRepository.Delete(path, CancellationToken.None);
            (await datalakeRepository.Exist(path, CancellationToken.None)).Should().BeFalse();
        }
    }
}
