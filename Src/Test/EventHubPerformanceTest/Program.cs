// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            IOption option = Option.Build(args);

            IWorkContext context = new WorkContextBuilder()
                .Set(cancellationTokenSource.Token)
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

            foreach(var item in actions.Where(x => x != null))
            {
                runningTasks.Add(item!.Run(context));
            }

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            Console.CancelKeyPress += delegate { cancellationTokenSource.Cancel(); };

            Console.WriteLine("Hit Ctrl C to quite");
            Console.WriteLine();

            await Task.WhenAll(runningTasks);
            return _ok;
        }
    }
}
