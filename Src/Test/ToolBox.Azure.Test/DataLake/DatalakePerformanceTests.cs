using FluentAssertions;
using Khoover.Toolbox.TestTools;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ToolBox.Azure.Test.DataLake
{
    public class DatalakePerformanceTests
    {
        private readonly AzureTestOption _testOption;
        private readonly ILoggerFactory _loggerFactory = new TestLoggerFactory();
        private readonly ITestOutputHelper _output;
        private int _totalCount;

        public DatalakePerformanceTests(ITestOutputHelper output)
        {
            _testOption = new TestOptionBuilder()
                .Build(new[] { "DatalakeOption:FileSystemName=test-performance" });

            _output = output;
        }

        [Fact]
        public async Task GivenTelemetryStreamModel_ForMultiplePartitions_ShouldPerform()
        {
            await InitializeFileSystem();

            const int max = 10;
            CancellationTokenSource tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            _totalCount = 0;

            Stopwatch sw = Stopwatch.StartNew();

            Task[] tasks = Enumerable.Range(0, max)
                .Select(x => RunPartition(x, tokenSource.Token))
                .ToArray();

            await Task.WhenAll(tasks);
            sw.Stop();

            _output.WriteLine($"Total count={_totalCount}, MS={sw.ElapsedMilliseconds}, Sec={sw.Elapsed.TotalSeconds}, TPS / {_totalCount / sw.Elapsed.TotalSeconds}");
        }

        private async Task InitializeFileSystem()
        {
            IDatalakeManagement management = _testOption.GetDatalakeManagement(_loggerFactory);
            await management.CreateIfNotExist(_testOption.DatalakeOption.FileSystemName, CancellationToken.None);

            IDatalakeRepository datalakeRepository = _testOption.GetDatalakeRepository(_loggerFactory);
            await ClearContainer(datalakeRepository);
        }

        private async Task RunPartition(int partition, CancellationToken token)
        {
            DateTime timestamp = DateTime.Now;

            IDatalakeRepository datalakeRepository = _testOption.GetDatalakeRepository(_loggerFactory);

            int count = 0;
            while (!token.IsCancellationRequested)
            {
                var data = new
                {
                    Partition = partition,
                    Timestamp = DateTime.Now,
                    Type = "trace",
                    RecordId = count,
                    Data = $"This is {count} record id"
                };
                count++;

                if (count % 100 == 0)
                {
                    timestamp = timestamp + TimeSpan.FromDays(1);
                }

                string json = Json.Default.Serialize(data) + Environment.NewLine;
                string path = $"telemetry/data/{timestamp.Year:D4}/{timestamp.Month:D2}/{timestamp.Day:D2}/{partition:D6}/trace.json";

                await datalakeRepository.Append(path, Encoding.UTF8.GetBytes(json), token);

                Interlocked.Increment(ref _totalCount);
            }
        }

        private async Task ClearContainer(IDatalakeRepository datalakeRepository)
        {
            await _testOption
                .GetDatalakeManagement(_loggerFactory)
                .CreateIfNotExist(_testOption.DatalakeOption.FileSystemName, CancellationToken.None);

            IReadOnlyList<DatalakePathItem> list = await datalakeRepository.Search(null!, x => true, false, CancellationToken.None);
            list.Should().NotBeNull();

            foreach (var fileItem in list.Where(x => x.IsDirectory == true))
            {
                await datalakeRepository.DeleteDirectory(fileItem.Name!, CancellationToken.None);
            }

            foreach (var fileItem in list.Where(x => x.IsDirectory == false))
            {
                await datalakeRepository.Delete(fileItem.Name!, CancellationToken.None);
            }
        }
    }
}
