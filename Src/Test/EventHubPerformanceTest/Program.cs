// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    class Program
    {
        private const int _ok = 0;
        private const int _error = 1;
        private const string _lifetimeScopeTag = "main";
        private readonly string _programTitle = $"Event Hub Performance Test - Version {Assembly.GetExecutingAssembly().GetName().Version}";

        static async Task<int> Main(string[] args)
        {
            try
            {
                return await new Program().Run(args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                DisplayStartDetails(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhanded exception: " + ex.ToString());
                DisplayStartDetails(args);
            }

            return _error;
        }

        private static void DisplayStartDetails(string[] args) => Console.WriteLine($"Arguments: {string.Join(", ", args)}");

        private async Task<int> Run(string[] args)
        {
            Console.WriteLine(_programTitle);
            Console.WriteLine();

            IOption option = Option.Build(args);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            using (ILifetimeScope container = CreateContainer(option).BeginLifetimeScope(_lifetimeScopeTag))
            {
                ITelemetry logger = container.Resolve<ITelemetry>();

                IWorkContext context = new WorkContextBuilder()
                .Set(cancellationTokenSource.Token)
                .Set(logger)
                .Set(new ServiceProviderProxy(x => container.Resolve(x)))
                .Build();

                if (option.Help)
                {
                    return _ok;
                }

                var actions = new IAction?[]
                {
                option.Send ? new SendEvents(option) : null,
                option.Receive ? new ReceiveEvents(option) : null,
                };

                var runningTasks = new List<Task>();

                foreach (var item in actions.Where(x => x != null))
                {
                    runningTasks.Add(item!.Run(context));
                }

                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    cancellationTokenSource.Cancel();

                    Console.WriteLine("Canceling...");
                };

                Console.WriteLine("Hit Ctrl C to quite");
                Console.WriteLine();

                await Task.WhenAll(runningTasks);
                return _ok;
            }
        }

        private IContainer CreateContainer(IOption option)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(option).As<IOption>();

            builder.Register(x => new ConsoleEventLogger()).As<ConsoleEventLogger>().InstancePerLifetimeScope();

            string logType = option.Send ? "Send" : (option.Receive ? "Receive" : "SendReceive");
            builder.Register(x => new FileEventLogger(option.LoggingFolder, logType)).As<FileEventLogger>().InstancePerLifetimeScope();

            builder.Register(x =>
                new TelemetryService()
                .AddConsoleLogger(option.ConsoleLevel, x.Resolve<ConsoleEventLogger>())
                .AddFileLogger(x.Resolve<FileEventLogger>())
                ).As<FileEventLogger>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
