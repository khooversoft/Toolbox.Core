using Azure;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public class DatalakeManagement : IDatalakeManagement
    {
        private readonly DataLakeServiceClient _serviceClient;
        private readonly ILogger<DatalakeManagement> _logger;

        public DatalakeManagement(DatalakeRepositoryOption azureStoreOption, ILogger<DatalakeManagement> logger)
        {
            _logger = logger;

            _logger = logger;
            _serviceClient = azureStoreOption.CreateDataLakeServiceClient();
        }

        public async Task<IReadOnlyList<string>> List(CancellationToken token)
        {
            var list = new List<string>();

            await foreach (FileSystemItem file in _serviceClient.GetFileSystemsAsync(cancellationToken: token))
            {
                list.Add(file.Name);
            }

            return list;
        }

        public async Task Create(string name, CancellationToken token)
        {
            name.VerifyNotEmpty(nameof(name));
            bool created = false;

            CancellationTokenSource tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            while (!tokenSource.IsCancellationRequested)
            {
                try
                {
                    await _serviceClient.CreateFileSystemAsync(name, cancellationToken: token);
                    created = true;
                    break;
                }
                catch (RequestFailedException ex) when (ex.ErrorCode != "ContainerBeingDeleted")
                {
                    throw;
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            while (!tokenSource.IsCancellationRequested)
            {
                IReadOnlyList<string> fileSystems = await List(token);
                if (fileSystems.SingleOrDefault(x => x == name) != null) return;

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            if (!created) throw new InvalidOperationException($"Could not create file system {name}");
        }

        public async Task Delete(string name, CancellationToken token)
        {
            name.VerifyNotEmpty(nameof(name));

            CancellationTokenSource tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            while (!tokenSource.IsCancellationRequested)
            {
                try
                {
                    await _serviceClient.DeleteFileSystemAsync(name, cancellationToken: token);
                    return;
                }
                catch (RequestFailedException ex) when (ex.ErrorCode != "ContainerBeingDeleted")
                {
                    throw;
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        public async Task CreateIfNotExist(string name, CancellationToken token)
        {
            IReadOnlyList<string> fileSystemNames = await List(token);
            if (fileSystemNames.SingleOrDefault(x => x == name) != null) return;

            await Create(name, token);
        }

        public async Task DeleteIfExist(string name, CancellationToken token)
        {
            IReadOnlyList<string> fileSystemNames = await List(token);
            if (fileSystemNames.SingleOrDefault(x => x == name) == null) return;

            await Delete(name, token);
        }
    }
}
