using Autofac;
using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SmartBlockServer
{
    class Program
    {
        private const int _ok = 0;
        private const int _error = 1;
        private const string _lifetimeScopeTag = "main";
        private readonly string _programTitle = $"Smart Block Server - Version {Assembly.GetExecutingAssembly().GetName().Version}";

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
                    .Set(new ServiceProviderProxySimple(x => container.Resolve(x)))
                    .Build();

                option
                    .FormatSettings()
                    .ForEach(x => context.Telemetry.Info(context, x));

                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    cancellationTokenSource.Cancel();

                    Console.WriteLine("Canceling...");
                };

                Console.WriteLine("Hit Ctrl C to quit");
                Console.WriteLine();

                await new IAction[]
                {
                    container.Resolve<RegisterRoute>(),
                    container.Resolve<ReceiveMessages>(),
                }
                .ForEachAsync(x => x.Run(context));

                return _ok;
            }
        }

        private IContainer CreateContainer(IOption option)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(option).As<IOption>();

            builder.RegisterType<RegisterRoute>().InstancePerLifetimeScope();
            builder.RegisterType<ReceiveMessages>().InstancePerLifetimeScope();

            var endpointRegistration = new ResourceEndpointRegistration(ResourceScheme.Queue, "SmartAgreement", option.ServiceBusConnection);

            builder.Register(x => new MessageNetClient(option.NodeId, new Uri(option.NameServerUri), endpointRegistration)).InstancePerLifetimeScope();

            BuildTelemetry(option, builder);

            builder.Register(x => x.Resolve<ITelemetryService>().CreateLogger("Main")).As<ITelemetry>().InstancePerLifetimeScope();

            return builder.Build();
        }

        private void BuildTelemetry(IOption option, ContainerBuilder builder)
        {
            builder.Register(x => new ConsoleEventLogger()).As<ConsoleEventLogger>().InstancePerLifetimeScope();

            string logType = "Server";
            builder.Register(x => new FileEventLogger(option.LoggingFolder!, logType)).As<FileEventLogger>().InstancePerLifetimeScope();

            Func<IComponentContext, ITelemetryService> telemetryService = x => new TelemetryService()
                    .AddConsoleLogger(option.ConsoleLevel, x.Resolve<ConsoleEventLogger>())
                    .AddFileLogger(x.Resolve<FileEventLogger>());

            builder.Register(x => telemetryService(x)).As<ITelemetryService>().InstancePerLifetimeScope();
        }
    }
}
