using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MessageHub.Management.Test
{
    [Collection("QueueTests")]
    public class GeneralQueueRegistrationTests
    {
        private readonly ServiceBusConnection _serviceBusConnection = new ServiceBusConnection("Endpoint=sb://messagehubtest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=COdoxUj4S71bFrClrJTbNY1IGSpPnVxERyTLFEOz58Q=;TransportType=Amqp");
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private readonly QueueManagement _queueManagement;

        private readonly QueueDefinition _queueDefinition = new QueueDefinition
        {
            QueueName = "Unit2_TestQueue2",
        };

        public GeneralQueueRegistrationTests()
        {
            _queueManagement = new QueueManagement(_serviceBusConnection);
        }

        [Fact]
        public async Task GivenQueueDefinition_WhenStateChange_ShouldCreateRemoveQueue()
        {
            bool exist = await _queueManagement.QueueExists(_workContext, _queueDefinition.QueueName);
            if (exist)
            {
                await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName);
            }

            // Act
            bool state = await new StateManagerBuilder()
                .Add(new CreateQueueState(_serviceBusConnection, _queueDefinition))
                .Build()
                .Set(_workContext);

            state.Should().BeTrue();

            IReadOnlyList<QueueDefinition> subjects = await _queueManagement.Search(_workContext, "unit2*");
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(1);
            subjects[0].QueueName.Should().BeEquivalentTo(_queueDefinition.QueueName);

            state = await new StateManagerBuilder()
                .Add(new RemoveQueueState(_serviceBusConnection, _queueDefinition.QueueName))
                .Build()
                .Set(_workContext);

            state.Should().BeTrue();

            subjects = await _queueManagement.Search(_workContext, "unit2*");
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(0);
        }
    }
}
