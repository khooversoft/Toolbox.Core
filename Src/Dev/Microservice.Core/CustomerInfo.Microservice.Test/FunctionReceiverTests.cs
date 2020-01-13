using Autofac;
using CustomerInfo.MicroService;
using FluentAssertions;
using Khooversoft.MessageNet.Client;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using Microservice.Interface.Test;
using MicroserviceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CustomerInfo.Microservice.Test
{
    public class FunctionReceiverTests
    {
        [Fact]
        public async Task GivenFunction_AfterBind_SendSingleMessageIsReceived()
        {
            IWorkContext workContext = WorkContext.Empty;
            ITestContext testContext = new TestContext();
            IAssemblyLoader assemblyLoader = new AssemblyLoader();
            MessageNetClientFake messageNetClient = new MessageNetClientFake();

            string assemblyPathToLoad = @"D:\Sources\Toolbox.Core\Src\Test\Microservice.Core\CustomerInfo.MicroService\bin\Debug\netstandard2.1\CustomerInfo.MicroService.dll";

            var diBuilder = new ContainerBuilder();
            diBuilder.RegisterInstance(testContext).As<ITestContext>();

            IReadOnlyList<FunctionConfiguration> functionConfigurations = assemblyLoader.LoadFromAssemblyPath(assemblyPathToLoad)
                .GetExportedTypes()
                .GetFunctionMethods()
                .GetFunctionConfiguration(new Uri("http://localhost"), "endpoint:fake.connections", new PropertyResolver())
                .RegisterTypesWithContainer(x => diBuilder.RegisterType(x));

            using ILifetimeScope lifetimeScope = diBuilder.Build().BeginLifetimeScope();

            IReadOnlyList<FunctionMessageReceiver> receivers = await functionConfigurations
                .Run(workContext, lifetimeScope, messageNetClient);

            string message = "Hello, my name is HAL";
            await messageNetClient.SendMessage(Encoding.UTF8.GetBytes(message));

            messageNetClient.Dispose();

            await receivers
                .ForEachAsync(x => x.Stop(workContext));

            testContext.MessageCount.Should().Be(1);
            testContext.Messages.Count.Should().Be(1);
            testContext.Messages.First().Should().Be(message);
        }

        //[Fact]
        //public async Task GivenFunction_AfterBind_SendMultpleMessageSIsReceived()
        //{
        //    ITestContext testContext = new TestContext();
        //    IAssemblyLoader assemblyLoader = new AssemblyLoader();

        //    string assemblyPathToLoad = @"D:\Sources\Toolbox.Core\Src\Test\Microservice.Core\CustomerInfo.MicroService\bin\Debug\netstandard2.1\CustomerInfo.MicroService.dll";

        //    Assembly assembly = assemblyLoader.LoadFromAssemblyPath(assemblyPathToLoad!);

        //    IReadOnlyList<FunctionConfiguration> functions = new ExecutionContextBuilder()
        //        .SetNameServerUri(new Uri("http://localhost"))
        //        .SetServiceBusConnection("endpoint:fake.connections")
        //        .SetTypes(assembly.GetExportedTypes())
        //        .Build(WorkContext.Empty);

        //    functions.Count.Should().Be(1);

        //    var diBuilder = new ContainerBuilder();
        //    diBuilder.RegisterInstance(testContext).As<ITestContext>();

        //    functions
        //        .ForEach(x => diBuilder.RegisterType(x.MethodInfo.DeclaringType!));

        //    using IContainer container = diBuilder.Build();

        //    IWorkContext workContext = new WorkContextBuilder()
        //        .Set(new ServiceProviderProxySimple(x => container.Resolve(x)))
        //        .Build();

        //    var messageNetClient = new MessageNetClientFake();
        //    FunctionMessageReceiver functionMessageReceiver = new FunctionMessageReceiver(executionContext.FunctionConfigurations[0], messageNetClient);

        //    await functionMessageReceiver.Start(workContext);

        //    string message = "Hello, my name is HAL";
        //    await messageNetClient.SendMessage(Encoding.UTF8.GetBytes(message));

        //    messageNetClient.Dispose();

        //    testContext.MessageCount.Should().Be(1);
        //    testContext.Messages.Count.Should().Be(1);
        //    testContext.Messages.First().Should().Be(message);
        //}
    }
}
