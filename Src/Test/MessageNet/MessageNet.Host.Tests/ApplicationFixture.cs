// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Autofac;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MessageNet.Host.Tests
{
    public class ApplicationFixture
    {
        private const string _connectionString = "Endpoint=sb://messagehubtest.servicebus.windows.net/;SharedAccessKeyName=TestAccess;SharedAccessKey={messagehub.accesskey};TransportType=Amqp";
        private ILifetimeScope _lifetimeScope;

        public ApplicationFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets("MessageNet.Host.Test")
                .Build();

            PropertyResolver = new PropertyResolver(configuration.GetChildren().ToDictionary(x => x.Key, x => x.Value));
        }

        public IPropertyResolver PropertyResolver { get; }

        public string ConnectionString => PropertyResolver.Resolve(_connectionString);

        public ILifetimeScope CreateContainer()
        {
            if (_lifetimeScope != null) return _lifetimeScope;

            NamespaceRegistration[] registrations = new NamespaceRegistration[]
            {
                new NamespaceRegistration("default", ConnectionString),
            };

            var config = new MessageNetConfig(registrations);
            var builder = new ContainerBuilder();

            builder.RegisterInstance(config).As<IMessageNetConfig>();
            builder.RegisterContainerModule(new MessageNetHostContainerRegistrations());

            _lifetimeScope = builder
                .Build()
                .BeginLifetimeScope("test");

            return _lifetimeScope;
        }
    }
}
