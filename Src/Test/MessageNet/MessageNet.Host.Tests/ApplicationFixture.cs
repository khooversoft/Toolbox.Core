// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MessageNet.Host.Tests
{
    public class ApplicationFixture
    {
        private const string _connectionString = "Endpoint=sb://messagehubtest.servicebus.windows.net/;SharedAccessKeyName=TestAccess;SharedAccessKey={messagehub.accesskey};TransportType=Amqp";
        private IMessageNetConfig _config;

        public ApplicationFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets("MessageNet.Host.Test")
                .Build();

            PropertyResolver = new PropertyResolver(configuration.GetChildren().ToDictionary(x => x.Key, x => x.Value));
        }

        public IPropertyResolver PropertyResolver { get; }

        public string ConnectionString => PropertyResolver.Resolve(_connectionString);

        public IMessageNetConfig GetMessageNetConfig() => _config ??= new MessageNetConfig(new NamespaceRegistration("default", ConnectionString));
    }
}
