// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.Standard
{
    public class FileEventLogger : ITelemetryLogger, IDisposable
    {
        private StreamWriter? _file;
        private Action<TelemetryMessage>? _outputAction;

        public FileEventLogger(string logFileName, bool useJsonFormat = false)
        {
            logFileName.VerifyNotEmpty(nameof(logFileName));

            LogFileName = logFileName;
            UseJsonFormat = useJsonFormat;
            Open(Path.GetDirectoryName(logFileName));
        }

        public FileEventLogger(string folderPath, string logType, bool useJsonFormat = false)
        {
            folderPath.VerifyNotEmpty(nameof(folderPath));
            logType.VerifyNotEmpty(nameof(logType));

            UseJsonFormat = useJsonFormat;

            Directory.CreateDirectory(folderPath);

            string extension = useJsonFormat ? "json" : "txt";

            // Only keep the last 20 logs
            new DirectoryInfo(folderPath)
                .GetFiles($"Log*.{extension}", SearchOption.TopDirectoryOnly)
                .OrderByDescending(x => x.LastWriteTime)
                .Skip(20)
                .ForEach(x => File.Delete(x.FullName));

            // Open new log file
            LogFileName = Path.Combine(folderPath, $"Log_{logType}_{Guid.NewGuid().ToString()}.{extension}");
            Open(folderPath);
        }

        public string LogFileName { get; }

        public bool UseJsonFormat { get; } = false;

        public void Write(TelemetryMessage message)
        {
            if (_file == null) return;

            _outputAction?.Invoke(message);
        }

        private void WriteTab(TelemetryMessage message)
        {
            var list = new string?[]
            {
                message.TelemetryType.ToString(),
                message.EventDate.ToString("yyMMdd:HH:mm:ss") + (message.EventDate.Offset.Hours >= 0 ? "+" : string.Empty) + message.EventDate.Offset.Hours.ToString(),
                message.WorkContext.ActivityId.ToString(),
                message.WorkContext.ParentActivityId.ToString(),
                message.EventSourceName,
                message.EventName,
                message.Message,
                message.Duration?.ToString(),
                message.Value?.ToString(),
                message.Exception?.ToString(),
                message.EventDimensions == null ? null : "{ " + string.Join(", ", message.EventDimensions.Select(x => x.Key + "=" + x.Value?.ToString())) + " }",
            };

            string line = string.Join("\t", list.Where(x => !x.IsEmpty()));

            _file?.WriteLine(line);
        }

        private void WriteJson(TelemetryMessage message)
        {
            _file?.WriteLine(message.ConvertToJson());
        }

        public void Dispose()
        {
            StreamWriter? fileStream = Interlocked.Exchange(ref _file, null);
            fileStream?.Close();
        }

        private void Open(string folderPath)
        {
            if (UseJsonFormat)
            {
                _outputAction = WriteJson;
            }
            else
            {
                _outputAction = WriteTab;
            }

            Directory.CreateDirectory(folderPath);

            _file = new StreamWriter(LogFileName, false);
            _file.AutoFlush = true;
        }
    }
}
