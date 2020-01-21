// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Khooversoft.Toolbox.Standard;
using System.Threading;
using Autofac;
using System.Collections.Generic;
using System.Linq;
using Khooversoft.MessageNet.Interface;
using Khooversoft.MessageNet.Client;

namespace ServiceBusPerformanceTest
{
    class Program
    {
        private const int _ok = 0;
        private const int _error = 1;
        private const string _lifetimeScopeTag = "main";
        private readonly string _programTitle = $"Service Bus Performance Test - Version {Assembly.GetExecutingAssembly().GetName().Version}";

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

            if (option.Help)
            {
                option.FormatHelp()
                    .ForEach(x => Console.WriteLine(x));

                return _ok;
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            using (ILifetimeScope container = CreateContainer(option).BeginLifetimeScope(_lifetimeScopeTag))
            {
                ITelemetry logger = container.Resolve<ITelemetry>();

                IWorkContext context = new WorkContextBuilder()
                    .Set(cancellationTokenSource.Token)
                    .Set(logger)
                    .Set(new ServiceProviderProxy(x => container.Resolve(x)))
                    .Build();

                option
                    .FormatSettings()
                    .ForEach(x => context.Telemetry.Info(context, x));

                List<Task> runningTasks = new IAction?[]
                {
                    option.Send ? container.Resolve<SendMessages>() : null,
                    option.Receive ? container.Resolve<ReceiveMessages>() : null,
                }
                .Where(x => x != null)
                .Select(x => Task.Run(() => x!.Run(context)))
                .ToList();

                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    cancellationTokenSource.Cancel();

                    Console.WriteLine("Canceling...");
                };

                Console.WriteLine("Hit Ctrl C to quit");
                Console.WriteLine();

                await Task.WhenAll(runningTasks);
                return _ok;
            }
        }

        private IContainer CreateContainer(IOption option)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(option).As<IOption>();

            builder.RegisterType<SendMessages>().InstancePerLifetimeScope();
            builder.RegisterType<ReceiveMessages>().InstancePerLifetimeScope();

            builder.RegisterType<MessageClientService>().As<IMessageClient>();
            builder.RegisterType<MessageQueueReceiveProcessor>().As<IMessageProcessor>();

            BuildTelemetry(option, builder);

            builder.Register(x => x.Resolve<ITelemetryService>().CreateLogger("Main")).As<ITelemetry>().InstancePerLifetimeScope();

            return builder.Build();
        }

        private void BuildTelemetry(IOption option, ContainerBuilder builder)
        {
            builder.Register(x => new ConsoleEventLogger()).As<ConsoleEventLogger>().InstancePerLifetimeScope();

            string logType = option.Send ? "Send" : (option.Receive ? "Receive" : "SendReceive");
            builder.Register(x => new FileEventLogger(option.LoggingFolder!, logType)).As<FileEventLogger>().InstancePerLifetimeScope();

            Func<IComponentContext, ITelemetryService> telemetryService = x => new TelemetryService()
                    .AddConsoleLogger(option.ConsoleLevel, x.Resolve<ConsoleEventLogger>())
                    .AddFileLogger(x.Resolve<FileEventLogger>());

            builder.Register(x => telemetryService(x)).As<ITelemetryService>().InstancePerLifetimeScope();


        }
    }
}
