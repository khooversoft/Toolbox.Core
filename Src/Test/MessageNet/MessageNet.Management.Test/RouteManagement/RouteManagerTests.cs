using Autofac;
using FluentAssertions;
using Khooversoft.MessageNet.Interface;
using Khooversoft.MessageNet.Management;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    .Set(new ServiceProviderProxy(x => container.Resolve(x), x => container.ResolveOptional(x)))
                    .Build();

                RouteManager manager = container.Resolve<RouteManager>();

                const string nodeId = "Need/Customer";

                RouteRegistrationRequest request = new RouteRegistrationRequest { NodeId = nodeId };

                RouteRegistrationResponse response = await manager.Register(_workContext, request);
                response.Should().NotBeNull();

                response.InputQueueUri.Should().NotBeNullOrEmpty();
                response.InputQueueUri.Should().Be(nodeId);

                RouteLookupRequest routeLookupRequest = new RouteLookupRequest
                {
                    SearchNodeId = nodeId,
                };

                IReadOnlyList<RouteLookupResponse>? routeLookupResponses = await manager.Search(_workContext, routeLookupRequest);
                routeLookupResponses.Should().NotBeNull();
                routeLookupResponses.Count.Should().Be(1);
                routeLookupResponses[0].NodeId.Should().Be(nodeId);
                routeLookupResponses[0].InputUri.Should().Be(nodeId);

                await manager.Unregister(_workContext, request);

                routeLookupResponses = await manager.Search(_workContext, routeLookupRequest);
                routeLookupResponses.Should().NotBeNull();
                routeLookupResponses.Count.Should().Be(0);
            }
        }

        [Fact]
        public async Task GivenThreeNode_WhenRegisterAndUnregistered_ShouldPass()
        {
            const int max = 1;
            IContainer rootContainer = CreateContainer();

            using (ILifetimeScope container = rootContainer.BeginLifetimeScope())
            {
                _workContext = new WorkContextBuilder()
                    .Set(new ServiceProviderProxy(x => container.Resolve(x), x => container.ResolveOptional(x)))
                    .Build();

                RouteManager manager = container.Resolve<RouteManager>();

                // Generate route request and expected URI
                Func<int, string> generateName = x => $"Need/Client_{x}";

                var routeRegistrations = Enumerable.Range(0, max)
                    .Select(x => new
                    {
                        NodeRigistration = new RouteRegistrationRequest { NodeId = generateName(x) },
                        NodeId = generateName(x)
                    })
                    .ToList();

                RouteRegistrationResponse[] responses = await routeRegistrations
                    .Select(x => manager.Register(_workContext, x.NodeRigistration))
                    .WhenAll();

                // Verify responses
                responses.Should().NotBeNull();
                responses.Length.Should().Be(max);

                responses.OrderBy(x => x.InputQueueUri)
                    .Zip(routeRegistrations.OrderBy(x => x.NodeId), (o, i) => (o, i))
                    .All(x => x.o.InputQueueUri == x.i.NodeId)
                    .Should().BeTrue();

                // Search for all nodes
                IReadOnlyList<RouteLookupResponse> routeLookupResponse = await manager.Search(_workContext, new RouteLookupRequest { SearchNodeId = "*" });
                routeLookupResponse.Should().NotBeNull();
                routeLookupResponse.Count.Should().Be(max);

                routeLookupResponse.OrderBy(x => x.InputUri)
                    .Zip(routeRegistrations.OrderBy(x => x.NodeId), (o, i) => (o, i))
                    .All(x => x.o.InputUri == x.i.NodeId)
                    .Should().BeTrue();

                // Unregister
                await routeRegistrations
                    .Select(x => manager.Unregister(_workContext, x.NodeRigistration))
                    .WhenAll();

                routeLookupResponse = await manager.Search(_workContext, new RouteLookupRequest { SearchNodeId = "*" });
                routeLookupResponse.Should().NotBeNull();
                routeLookupResponse.Count.Should().Be(0);
            }
        }

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
            builder.RegisterType<RouteManager>().As<RouteManager>().InstancePerLifetimeScope();

            builder.Register(x => new BlobStoreConnection("Default", "ConnectionString")).As<BlobStoreConnection>().InstancePerLifetimeScope();
            builder.Register(x => new ServiceBusConnection("Endpoint=sb://messagehubtest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={key};TransportType=Amqp")).As<ServiceBusConnection>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
