// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class ZipContainerReader : IDisposable
    {
        private ZipArchive? _zipArchive;

        public ZipContainerReader(ZipArchive zipArchive)
        {
            zipArchive.Verify(nameof(zipArchive)).IsNotNull();

            _zipArchive = zipArchive;
        }

        public ZipContainerReader(string filePath)
        {
            filePath.Verify(nameof(filePath)).IsNotEmpty();

            FilePath = filePath;
        }

        public string? FilePath { get; }

        public ZipContainerReader OpenFile(IWorkContext context)
        {
            _zipArchive.Verify().Assert(x => x == null, "Zip archive already opened");
            File.Exists(FilePath).Verify(nameof(FilePath)).Assert(x => x == true, $"{FilePath} does not exist");

            context.Telemetry.Info(context, $"Reading {FilePath}");
            _zipArchive = ZipFile.Open(FilePath, ZipArchiveMode.Read);

            return this;
        }

        public void Close()
        {
            var archive = Interlocked.Exchange(ref _zipArchive, null!);
            archive?.Dispose();
        }

        public bool Exist(IWorkContext context, string zipPath)
        {
            zipPath.Verify(nameof(zipPath)).IsNotEmpty();

            ZipArchiveEntry zipArchiveEntry = _zipArchive!.Entries
                .Where(x => x.FullName == zipPath).FirstOrDefault();

            return zipArchiveEntry != null;
        }

        public string Read(IWorkContext context, string zipPath)
        {
            var memoryStream = new MemoryStream();
            Read(context, zipPath, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        public ZipContainerReader Read(IWorkContext context, string zipPath, Stream targetStream)
        {
            context.Verify(nameof(context)).IsNotNull();
            zipPath.Verify(nameof(zipPath)).IsNotEmpty();
            targetStream.Verify(nameof(targetStream)).IsNotNull();
            _zipArchive.Verify().IsNotNull("Not opened");

            ZipArchiveEntry? entry = _zipArchive!.GetEntry(zipPath);
            entry.Verify().IsNotNull($"{zipPath} does not exist in zip");

            using StreamReader writer = new StreamReader(entry.Open());
            writer.BaseStream.CopyTo(targetStream);

            return this;
        }

        public void Dispose()
        {
            Close();
        }

    }
}
