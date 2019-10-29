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

        private static void DisplayStartDetails(string[] args)
        {
            Console.WriteLine($"Arguments: {string.Join(", ", args)}");
        }

        private async Task<int> Run(string[] args)
        {
            Console.WriteLine(_programTitle);
            Console.WriteLine();

            IOption option = Option.Build(args);

            if (option.Help)
            {
                return _ok;
            }

            var tasks = new Func<Task>[]
            {
                option.Send ? () => new SendEvents(option).Run() : (Func<Task>)null,
                option.Receive ? () => new ReceiveEvents(option).Run() : (Func<Task>)null,
            };

            var runningTasks = new List<Task>();

            foreach(var item in tasks.Where(x => x != null))
            {
                runningTasks.Add(Task.Run(item));
            }

            Console.CancelKeyPress += delegate { option.CancellationTokenSource.Cancel(); };

            Console.WriteLine("Hit Ctrl C to quite");
            Console.WriteLine();

            await Task.WhenAll(runningTasks);
            return _ok;
        }
    }
}
