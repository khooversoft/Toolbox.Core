using Azure;
using Azure.Storage;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public class DatalakeRepository : IDatalakeRepository
    {
        private readonly DataLakeServiceClient _serviceClient;
        private readonly DataLakeFileSystemClient _fileSystem;
        private readonly ILogger<DatalakeRepository> _logger;

        public DatalakeRepository(DatalakeRepositoryOption azureStoreOption, ILogger<DatalakeRepository> logger)
        {
            azureStoreOption.VerifyNotNull(nameof(azureStoreOption)).Verify();
            logger.VerifyNotNull(nameof(logger));

            _logger = logger;
            _serviceClient = azureStoreOption.CreateDataLakeServiceClient();

            // Get a reference to a file system (container)
            _fileSystem = _serviceClient.GetFileSystemClient(azureStoreOption.FileSystemName);
        }

        public async Task Delete(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            _logger.LogTrace($"{nameof(Delete)} deleting {path}");

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            await file.DeleteIfExistsAsync(cancellationToken: token);
        }

        public async Task Download(string path, Stream toStream, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));
            toStream.VerifyNotNull(nameof(toStream));

            _logger.LogTrace($"{nameof(Download)} downloading {path} to stream");

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            await file.ReadToAsync(toStream, cancellationToken: token);
        }

        public async Task Upload(Stream fromStream, string toPath, bool force, CancellationToken token)
        {
            fromStream.VerifyNotNull(nameof(fromStream));
            toPath.VerifyNotEmpty(nameof(toPath));

            _logger.LogTrace($"{nameof(Upload)} from stream to {toPath}");

            DataLakeFileClient file = _fileSystem.GetFileClient(toPath);
            await file.UploadAsync(fromStream, force, token);
        }

        public async Task<DatalakePathProperties> GetPathProperties(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            return (await file.GetPropertiesAsync(cancellationToken: token)).Value
                .ConvertTo();
        }

        public async Task<bool> Exist(string path, CancellationToken token)
        {
            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            Response<bool> response = await file.ExistsAsync(token);
            return response.Value;
        }

        public async Task<IReadOnlyList<DatalakePathItem>> Search(string? path, Func<DatalakePathItem, bool> filter, bool recursive, CancellationToken token)
        {
            var list = new List<DatalakePathItem>();

            await foreach (PathItem pathItem in _fileSystem.GetPathsAsync(path, recursive, cancellationToken: token))
            {
                DatalakePathItem datalakePathItem = pathItem.ConvertTo();

                if (filter(datalakePathItem)) list.Add(datalakePathItem);
            }

            return list;
        }

        public async Task<byte[]> Read(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            Response<FileDownloadInfo> response = await file.ReadAsync(token);

            _logger.LogTrace($"{nameof(Read)} from {path}");

            using MemoryStream memory = new MemoryStream();
            await response.Value.Content.CopyToAsync(memory);

            return memory.ToArray();
        }

        public async Task Write(string path, byte[] data, bool force, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));
            data
                .VerifyNotNull(nameof(data))
                .VerifyAssert(x => x.Length > 0, $"{nameof(data)} length must be greater then 0");

            _logger.LogTrace($"{nameof(Write)} to {path}");
            using var memoryBuffer = new MemoryStream(data.ToArray());

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            await file.UploadAsync(memoryBuffer, force, token);
        }

        public async Task DeleteDirectory(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            _logger.LogTrace($"{nameof(DeleteDirectory)} {path}");

            DataLakeDirectoryClient directoryClient = _fileSystem.GetDirectoryClient(path);
            await directoryClient.DeleteAsync(cancellationToken: token);
        }
    }
}
