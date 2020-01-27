using Autofac.Extensions.DependencyInjection;
using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Management;
using MessageHub.NameServer;
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

namespace MessageNet.Node.Tests
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
