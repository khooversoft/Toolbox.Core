﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public class BlobRepository : IBlobRepository
    {
        private readonly BlobStoreConnection _blobStoreConnection;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;

        public BlobRepository(BlobStoreConnection blobStoreConnection)
        {
            blobStoreConnection.Verify(nameof(blobStoreConnection)).IsNotNull();

            _blobStoreConnection = blobStoreConnection;
            _blobServiceClient = new BlobServiceClient(_blobStoreConnection.ConnectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_blobStoreConnection.ContainerName);
        }

        public async Task CreateContainer(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();

            int count = 0;
            const int max = 10;
            RequestFailedException? save = null;

            while (count++ < max)
            {
                try
                {
                    await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: context.CancellationToken);
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

        public Task DeleteContainer(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();

            return _containerClient.DeleteIfExistsAsync(cancellationToken: context.CancellationToken);
        }

        public async Task Set(IWorkContext context, string path, string data)
        {
            data.Verify(nameof(data)).IsNotEmpty();

            using Stream content = new MemoryStream(Encoding.UTF8.GetBytes(data));
            await Upload(context, path, content);
        }

        public async Task Upload(IWorkContext context, string path, IEnumerable<byte> content)
        {
            context.Verify(nameof(context)).IsNotNull();
            path.Verify(nameof(path)).IsNotEmpty();
            content.Verify(nameof(content)).IsNotNull();

            using var memoryBuffer = new MemoryStream(content.ToArray());
            await _containerClient.UploadBlobAsync(path, memoryBuffer, context.CancellationToken);
        }

        public async Task Upload(IWorkContext context, string path, Stream content)
        {
            context.Verify(nameof(context)).IsNotNull();
            path.Verify(nameof(path)).IsNotEmpty();
            content.Verify(nameof(content)).IsNotNull();

            await _containerClient.UploadBlobAsync(path, content, context.CancellationToken);
        }

        public async Task<string> Get(IWorkContext context, string path)
        {
            path.Verify(nameof(path)).IsNotEmpty();

            IReadOnlyList<byte> blob = await Download(context, path);
            return Encoding.UTF8.GetString(blob.ToArray(), 0, blob.Count);
        }

        public async Task<IReadOnlyList<byte>> Download(IWorkContext context, string path)
        {
            context.Verify(nameof(context)).IsNotNull();
            path.Verify(nameof(path)).IsNotEmpty();

            BlobClient blobClient = _containerClient.GetBlobClient(path);
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using MemoryStream memory = new MemoryStream();
            using var writer = new StreamWriter(memory);

            await download.Content.CopyToAsync(memory);
            writer.Flush();
            memory.Position = 0;

            return memory.ToArray();
        }

        public Task Delete(IWorkContext context, string path)
        {
            context.Verify(nameof(context)).IsNotNull();
            path.Verify(nameof(path)).IsNotEmpty();

            return _containerClient.DeleteBlobIfExistsAsync(path, cancellationToken: context.CancellationToken);
        }

        public async Task<IReadOnlyList<string>> List(IWorkContext context, string search)
        {
            var list = new List<string>();

            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync())
            {
                if (SearchCompare(search, blobItem.Name)) list.Add(blobItem.Name);
            }

            return list;
        }

        public async Task ClearAll(IWorkContext context)
        {
            var list = await List(context, "*");

            foreach (var item in list)
            {
                await Delete(context, item);
            }
        }

        public bool SearchCompare(string search, string name)
        {
            if (search == "*") return true;

            return search.Equals(name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
