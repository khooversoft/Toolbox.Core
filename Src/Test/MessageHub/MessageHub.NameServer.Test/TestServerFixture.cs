using Autofac.Extensions.DependencyInjection;
using Khooversoft.MessageHub.Interface;
using Khooversoft.MessageHub.Management;
using Khooversoft.Toolbox.Standard;
using MessageHub.Client;
using MessageHub.Management.Test.RouteManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageHub.NameServer.Test
{
    public class TestServerFixture : IDisposable
    {
        private TestServer? _testServer;
        private IHost _host;
        private HttpClient? _client;
        private NameServerClient _nameServerClient;

        public HttpClient Client => _client ?? throw new ArgumentNullException(nameof(Client));

        public NameServerClient NameServerClient => _nameServerClient ?? throw new ArgumentNullException(nameof(NameServerClient));

        public TestServerFixture()
        {
            var host = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureHostConfiguration(config =>
                {
                    config
                        .AddUserSecrets("MessageHub.NameServer.Test");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton<IBlobRepository, BlobRepository>()
                        .AddSingleton<IQueueManagement, QueueManagement>()
                        .AddSingleton<IRegisterStore, BlobStore>();

                    //services.AddSingleton<IBlobRepository, BlobRepository>();
                    //services.AddSingleton<IQueueManagement, QueueManagementFake>();
                    //services.AddSingleton<IRegisterStore, BlobStore>();
                    //services.AddSingleton(x => new BlobStoreConnection("Default", "ConnectionString"));
                    //services.AddSingleton(x => new ServiceBusConnection("Endpoint=sb://messagehubtest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={key};TransportType=Amqp"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>();
                });

            _host = host.Start();

            _client = _host.GetTestServer().CreateClient();
            _nameServerClient = new NameServerClient(Client);
        }

        public void Dispose()
        {
            HttpClient? httpClient = Interlocked.Exchange(ref _client, null);
            httpClient?.Dispose();

            TestServer? testServer = Interlocked.Exchange(ref _testServer, null);
            testServer?.Dispose();
        }
    }
}
