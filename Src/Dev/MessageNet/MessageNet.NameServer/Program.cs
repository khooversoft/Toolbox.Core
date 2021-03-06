using Autofac.Extensions.DependencyInjection;
using Khooversoft.MessageNet.Management;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessageHub.NameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureHostConfiguration(config =>
                {
                    config
                        .AddUserSecrets<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton<IBlobRepository, BlobRepository>()
                        .AddSingleton<IQueueManagement, QueueManagement>()
                        .AddSingleton<IRegisterStore, BlobStore>();
                })                    
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
