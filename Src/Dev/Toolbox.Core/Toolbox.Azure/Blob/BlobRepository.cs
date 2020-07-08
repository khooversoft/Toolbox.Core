// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public class BlobRepository : IBlobRepository
    {
        private readonly BlobRepositoryOption _option;
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<BlobRepository> _logger;

        public BlobRepository(BlobRepositoryOption blobOption, ILogger<BlobRepository> logger)
        {
            blobOption.VerifyNotNull(nameof(blobOption)).Verify();
            logger.VerifyNotNull(nameof(logger));

            _option = blobOption;
            _logger = logger;

            var client = new BlobServiceClient(blobOption.GetResolvedConnectionString());
            _containerClient = client.GetBlobContainerClient(blobOption.ContainerName);
        }

        public async Task CreateContainer(CancellationToken token)
        {
            int count = 0;
            const int max = 10;
            RequestFailedException? save = null;

            while (count++ < max)
            {
                try
                {
                    _logger.LogTrace($"Create container {_option.ContainerName}");
                    await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: token);
                    return;
                }
                catch (RequestFailedException ex)
                {
                    save = ex;
                    await Task.Delay(TimeSpan.FromMilliseconds(count * 500));
                }
            }

            throw save!;
        }

        public async Task DeleteContainer(CancellationToken token)
        {
            _logger.LogTrace($"Delete container {_option.ContainerName}");
            await _containerClient.DeleteIfExistsAsync(cancellationToken: token);
        }

        public async Task<bool> Exists(CancellationToken token) => (await _containerClient.ExistsAsync(token)).Value;

        public async Task Set(string path, string data, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));
            data.VerifyNotEmpty(nameof(data));

            _logger.LogTrace($"{nameof(Set)} - uploading blob to {path}");
            using Stream content = new MemoryStream(Encoding.UTF8.GetBytes(data));
            await _containerClient.UploadBlobAsync(path, content, token);
        }

        public Task Set<T>(string path, T data, CancellationToken token) where T : class
        {
            path.VerifyNotEmpty(nameof(path));
            data.VerifyNotNull(nameof(data));

            _logger.LogTrace($"{nameof(Set)}:{typeof(T).Name} - uploading blob to {path}");
            string subject = JsonConvert.SerializeObject(data);
            return Set(path, subject, token);
        }

        public async Task<string> Get(string path)
        {
            path.VerifyNotEmpty(nameof(path));

            BlobClient blobClient = _containerClient.GetBlobClient(path);
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (MemoryStream memory = new MemoryStream())
            using (var writer = new StreamWriter(memory))
            {
                await download.Content.CopyToAsync(memory);
                writer.Flush();
                memory.Position = 0;

                using StreamReader reader = new StreamReader(memory);
                return reader.ReadToEnd();
            }
        }

        public async Task<T> Get<T>(string path) where T : class
        {
            string data = await Get(path);
            return JsonConvert.DeserializeObject<T>(data);
        }

        public Task Delete(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            _logger.LogTrace($"Deleting {path}");
            return _containerClient.DeleteBlobIfExistsAsync(path, cancellationToken: token);
        }

        public async Task<IReadOnlyList<string>> List(string search)
        {
            var list = new List<string>();

            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync())
            {
                if (SearchCompare(search, blobItem.Name)) list.Add(blobItem.Name);
            }

            return list;
        }

        public async Task Upload(string path, IEnumerable<byte> content, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));
            content.VerifyNotNull(nameof(content));

            _logger.LogTrace($"{nameof(Upload)}:Array - uploading blob to {path}");

            using var memoryBuffer = new MemoryStream(content.ToArray());
            await _containerClient.UploadBlobAsync(path, memoryBuffer, token);
        }

        public async Task Upload(string path, Stream content, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));
            content.VerifyNotNull(nameof(content));

            _logger.LogTrace($"{nameof(Upload)}:Stream - uploading blob to {path}");
            await _containerClient.UploadBlobAsync(path, content, token);
        }

        public async Task<IReadOnlyList<byte>> Download(string path)
        {
            path.VerifyNotEmpty(nameof(path));

            BlobClient blobClient = _containerClient.GetBlobClient(path);
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            _logger.LogTrace($"{nameof(Download)}:Array - downloading blob to {path}");

            using MemoryStream memory = new MemoryStream();
            using var writer = new StreamWriter(memory);

            await download.Content.CopyToAsync(memory);
            writer.Flush();
            memory.Position = 0;

            return memory.ToArray();
        }

        public async Task ClearAll(CancellationToken token)
        {
            var list = await List("*");

            _logger.LogTrace($"{nameof(ClearAll)} - removing all blobs");

            foreach (var item in list)
            {
                await Delete(item, token);
            }
        }

        public bool SearchCompare(string search, string name)
        {
            if (search == "*") return true;

            return search.Equals(name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
