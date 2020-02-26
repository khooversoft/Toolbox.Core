// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using FluentAssertions;
using Khooversoft.MessageNet.Host;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Autofac;
using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MessageNet.Management.Test.RouteManagement
{
    [Collection("QueueTests")]
    public class RouteManagerTests
    {
        private static IWorkContext? _workContext;

        [Fact]
        public async Task GivenNode_WhenRegisterAndUnregistered_ShouldPass()
        {
            IContainer rootContainer = CreateContainer();

            using (ILifetimeScope container = rootContainer.BeginLifetimeScope())
            {
                _workContext = new WorkContextBuilder()
                    .Set(new ServiceContainerBuilder().SetLifetimeScope(container).Build())
                    .Build();

                RouteRepository repository = container.Resolve<RouteRepository>();

                var queueId = new QueueId("default", "development", "Need.Customer");
                RouteRequest request = new RouteRequest { NetworkId = queueId.NetworkId, NodeId = queueId.NodeId };

                QueueId response = await repository.Register(_workContext, request);
                response.Should().NotBeNull();

                response.NetworkId.Should().Be(queueId.NetworkId);
                response.NodeId.Should().Be(queueId.NodeId);

                IReadOnlyList<QueueId>? routeLookupResponses = await repository.Search(_workContext, "*");
                routeLookupResponses.Should().NotBeNull();
                routeLookupResponses.Count.Should().Be(1);
                routeLookupResponses[0].Namespace.Should().NotBeNullOrWhiteSpace();
                routeLookupResponses[0].NetworkId.Should().Be(queueId.NetworkId);
                routeLookupResponses[0].NodeId.Should().Be(queueId.NodeId);

                await repository.Unregister(_workContext, request);

                routeLookupResponses = await repository.Search(_workContext, "*");
                routeLookupResponses.Should().NotBeNull();
                routeLookupResponses.Count.Should().Be(0);
            }
        }

        [Fact]
        //public async Task GivenThreeNode_WhenRegisterAndUnregistered_ShouldPass()
        //{
        //    const int max = 1;
        //    IContainer rootContainer = CreateContainer();

        //    using (ILifetimeScope container = rootContainer.BeginLifetimeScope())
        //    {
        //        _workContext = new WorkContextBuilder()
        //            .Set(new ServiceContainerBuilder().SetLifetimeScope(container).Build())
        //            .Build();

        //        RouteManager manager = container.Resolve<RouteManager>();

        //        // Generate route request and expected URI
        //        Func<int, string> generateName = x => $"Need/Client_{x}";

        //        var routeRegistrations = Enumerable.Range(0, max)
        //            .Select(x => new
        //            {
        //                NodeRigistration = RouteRegistrationRequest.Test(generateName(x)),
        //                NodeId = generateName(x)
        //            })
        //            .ToList();

        //        RouteRegistrationResponse[] responses = await routeRegistrations
        //            .Select(x => manager.Register(_workContext, x.NodeRigistration))
        //            .WhenAll();

        //        // Verify responses
        //        responses.Should().NotBeNull();
        //        responses.Length.Should().Be(max);

        //        responses.OrderBy(x => x.InputQueueUri)
        //            .Zip(routeRegistrations.OrderBy(x => x.), (o, i) => (o, i))
        //            .All(x => x.o.InputQueueUri == x.i.NodeId)
        //            .Should().BeTrue();

        //        // Search for all nodes
        //        IReadOnlyList<RouteLookupResponse> routeLookupResponse = await manager.Search(_workContext, new RouteLookupRequest { NodeId = "*" });
        //        routeLookupResponse.Should().NotBeNull();
        //        routeLookupResponse.Count.Should().Be(max);

        //        routeLookupResponse.OrderBy(x => x.InputUri)
        //            .Zip(routeRegistrations.OrderBy(x => x.NodeId), (o, i) => (o, i))
        //            .All(x => x.o.InputUri == x.i.NodeId)
        //            .Should().BeTrue();

        //        // Unregister
        //        await routeRegistrations
        //            .Select(x => manager.Unregister(_workContext, x.NodeRigistration))
        //            .WhenAll();

        //        routeLookupResponse = await manager.Search(_workContext, new RouteLookupRequest { NodeId = "*" });
        //        routeLookupResponse.Should().NotBeNull();
        //        routeLookupResponse.Count.Should().Be(0);
        //    }
        //}

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<BlobRepositoryFake>().As<IBlobRepository>().InstancePerLifetimeScope();
            builder.RegisterType<QueueManagementFake>().As<IQueueManagement>().InstancePerLifetimeScope();
            builder.RegisterType<BlobStore>().As<IRegisterStore>().InstancePerLifetimeScope();

            builder.RegisterType<NodeRegistrationActor>().As<INodeRegistrationActor>();
            builder.RegisterType<QueueManagementActor>().As<IQueueManagementActor>();
            builder.RegisterType<NodeRegistrationManagementActor>().As<INodeRegistrationManagementActor>();

            builder.Register(x => new ActorConfigurationBuilder().Set(_workContext!).Build()).As<ActorConfiguration>().InstancePerLifetimeScope();

            builder.RegisterType<ActorManager>().As<IActorManager>().InstancePerLifetimeScope();
            builder.RegisterType<RouteRepository>().As<RouteRepository>().InstancePerLifetimeScope();

            builder.Register(x => new BlobStoreConnection("Default", "ConnectionString")).As<BlobStoreConnection>().InstancePerLifetimeScope();
            builder.Register(x => new ServiceBusConnection("Endpoint=sb://messagehubtest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={key};TransportType=Amqp")).As<ServiceBusConnection>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
