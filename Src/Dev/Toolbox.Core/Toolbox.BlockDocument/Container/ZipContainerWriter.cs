// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class ZipContainerWriter : IDisposable
    {
        private ZipArchive? _zipArchive;

        public ZipContainerWriter(ZipArchive zipArchive)
        {
            zipArchive.Verify(nameof(zipArchive)).IsNotNull();

            _zipArchive = zipArchive;
        }

        public ZipContainerWriter(string filePath)
        {
            filePath.Verify(nameof(filePath)).IsNotEmpty();

            FilePath = filePath;
        }

        public string? FilePath { get; }

        public ZipContainerWriter OpenFile(IWorkContext context)
        {
            _zipArchive.Verify().Assert(x => x == null, "Zip archive already opened");

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            if (File.Exists(FilePath))
            {
                context.Telemetry.Info(context, $"Deleting {FilePath}");
                File.Delete(FilePath);
            }

            context.Telemetry.Info(context, $"Creating {FilePath}");
            _zipArchive = ZipFile.Open(FilePath, ZipArchiveMode.Create);

            return this;
        }

        public void Close()
        {
            var archive = Interlocked.Exchange(ref _zipArchive, null!);
            archive?.Dispose();
        }

        public ZipContainerWriter Write(IWorkContext context, string zipPath, string data)
        {
            data.Verify(nameof(data)).IsNotEmpty();

            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            return Write(context, zipPath, memoryStream);
        }

        public ZipContainerWriter Write(IWorkContext context, string zipPath, Stream sourceStream)
        {
            context.Verify(nameof(context)).IsNotNull();
            zipPath.Verify(nameof(zipPath)).IsNotEmpty();
            sourceStream.Verify(nameof(sourceStream)).IsNotNull();
            _zipArchive.Verify().IsNotNull("Not opened");

            ZipArchiveEntry entry = _zipArchive!.CreateEntry(zipPath);

            using StreamWriter writer = new StreamWriter(entry.Open());
            sourceStream.CopyTo(writer.BaseStream);

            return this;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
