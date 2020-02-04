using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public class LocalProcess
    {
        private List<string>? _outputList;
        private object _lock = new object();

        public LocalProcess()
        {
            CreateNoWindow = true;
            RemoveBlankLine = true;
        }

        public bool UseShellExecute { get; set; }

        public bool CreateNoWindow { get; set; }

        public int? SuccessExitCode { get; set; }

        public bool RemoveBlankLine { get; set; }

        public bool CaptureOutput { get; set; }

        public string? File { get; set; }

        public string? Arguments { get; set; }

        public string? WorkingDirectory { get; set; }

        public bool WhatIf { get; set; }

        public int? ExitCode { get; private set; }

        public Process? Process { get; private set; }

        public bool IsRunning => Process?.HasExited == false;

        public IReadOnlyList<string>? Output => _outputList;

        public LocalProcess SetUseShellExecute(bool value) { UseShellExecute = value; return this; }

        public LocalProcess SetCreateNoWindow(bool value) { CreateNoWindow = value; return this; }

        public LocalProcess SetSuccessExitCode(int value) { SuccessExitCode = value; return this; }

        public LocalProcess SetRemoveBlankLine(bool value) { RemoveBlankLine = value; return this; }

        public LocalProcess SetFile(string value) { File = value.Verify(nameof(value)).IsNotEmpty().Value; return this; }

        public LocalProcess SetArguments(string value) { Arguments = value; return this; }

        public LocalProcess SetWorkingDirectory(string value) { WorkingDirectory = value; return this; }

        public LocalProcess SetCaptureOutput(bool value) { CaptureOutput = value; return this; }

        public LocalProcess SetWhatIf(bool value) { WhatIf = value; return this; }

        public static Task<LocalProcess> Run(IWorkContext context, string file, string arguments, string workingDirectory = null)
        {
            return new LocalProcess()
                .SetFile(file)
                .SetArguments(arguments)
                .SetWorkingDirectory(workingDirectory)
                .Start(context);
        }

        public Task RunInBackground(IWorkContext context)
        {
            return Task.Run(() => Start(context));
        }

        public void Cancel()
        {
            if (!IsRunning)
            {
                return;
            }

            Process?.Kill();
            Process?.Close();
        }

        public Task<LocalProcess> Start(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            File!.Verify(nameof(File)).IsNotEmpty();

            _outputList = CaptureOutput ? new List<string>() : null;

            Process = new Process()
            {
                EnableRaisingEvents = true,

                StartInfo = new ProcessStartInfo
                {
                    FileName = File,
                    Arguments = !Arguments.IsEmpty() ? Arguments : null,
                    WorkingDirectory = !WorkingDirectory.IsEmpty() ? WorkingDirectory : null,

                    UseShellExecute = UseShellExecute,
                    CreateNoWindow = CreateNoWindow,
                }
            };

            context.Telemetry.Verbose(context, $"{nameof(LocalProcess)}: Starting local process, File={File}, Arguments={Arguments}, WorkingDirectory={WorkingDirectory}");

            if (WhatIf)
            {
                context.Telemetry.Verbose(context, $"{nameof(LocalProcess)}: WhatIf=true");
                ExitCode = 0;
                return Task.FromResult(this);
            }

            if (!UseShellExecute)
            {
                Process.StartInfo.RedirectStandardOutput = true;
                Process.OutputDataReceived += (s, e) => LogOutput(context, false, e.Data);

                Process.StartInfo.RedirectStandardError = true;
                Process.ErrorDataReceived += (s, e) => LogOutput(context, true, e.Data);
            }

            var tcs = new TaskCompletionSource<LocalProcess>();
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);

            Process.Exited += (sender, args) =>
            {
                switch (Process.ExitCode)
                {
                    case int v when v != (SuccessExitCode ?? 0):
                        var errorMessage = Process.StandardError.ReadToEnd();
                        tcs.SetException(new ArgumentException($"{nameof(LocalProcess)}: Exit code: {ExitCode} does not match required exit code {SuccessExitCode}, ErrorMessage={errorMessage}"));
                        break;

                    default:
                        Process.Close();
                        Process.Dispose();
                        tcs.SetResult(this);
                        break;
                }
            };

            linkedTokenSource.Token.Register(() =>
            {
                context.Telemetry.Verbose(context, $"{nameof(LocalProcess)}: Canceled local process, File={File}");
                Cancel();
                tcs.SetResult(this);
            });

            // Start process
            Process.Start()
                .Verify()
                .Assert(x => x == true, $"{nameof(LocalProcess)}: Could not start process");

            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();

            return tcs.Task;
        }

        private void LogOutput(IWorkContext context, bool error, string data)
        {
            if (data == null)
            {
                return;
            }

            string msg = $"LocalProcess{(error ? " (error)" : string.Empty)}: {data}";
            context.Telemetry.Verbose(context, msg);

            lock (_lock)
            {
                _outputList?.Add(data);
            }
        }
    }
}
