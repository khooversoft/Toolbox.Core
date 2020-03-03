using FluentAssertions;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ToolBox.Azure.Test.Queue
{
    [Collection("QueueTests")]
    public class GeneralQueueRegistrationTests : IClassFixture<ApplicationFixture>
    {
        private readonly IWorkContext _workContext = WorkContextBuilder.Default;
        private readonly QueueManagement _queueManagement;
        private readonly QueueDefinition _queueDefinition = new QueueDefinition("Unit2_TestQueue2");

        public GeneralQueueRegistrationTests(ApplicationFixture application)
        {
            _queueManagement = new QueueManagement(application.ConnectionString);
        }

        [Fact]
        public async Task GivenQueueDefinition_WhenStateChange_ShouldCreateRemoveQueue()
        {
            bool exist = await _queueManagement.QueueExists(_workContext, _queueDefinition.QueueName!);
            if (exist)
            {
                await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);
            }

            // Act
            bool state = await new StateManagerBuilder()
                .Add(new CreateQueueState(_queueManagement, _queueDefinition))
                .Build()
                .Set(_workContext);

            state.Should().BeTrue();

            IReadOnlyList<QueueDefinition> subjects = await _queueManagement.Search(_workContext, "unit2*");
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(1);
            subjects[0].QueueName.Should().BeEquivalentTo(_queueDefinition.QueueName);

            state = await new StateManagerBuilder()
                .Add(new RemoveQueueState(_queueManagement, _queueDefinition.QueueName!))
                .Build()
                .Set(_workContext);

            state.Should().BeTrue();

            subjects = await _queueManagement.Search(_workContext, "unit2*");
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(0);
        }

        [Theory]
        [InlineData("namespace")]
        [InlineData("namespace/nodeid")]
        [InlineData("namespace/service/nodeid")]
        [InlineData("namespace/service-system/nodeid")]
        public async Task GivenQueuePAth_WhenCreated_ShouldNotThrow(string nodeId)
        {
            IReadOnlyList<QueueDefinition> subjects = await _queueManagement.Search(_workContext);
            foreach (var item in subjects)
            {
                await _queueManagement.DeleteQueue(_workContext, item.QueueName!);
            }

            var testDefinition = new QueueDefinition(nodeId);

            bool exist = await _queueManagement.QueueExists(_workContext, testDefinition.QueueName);
            if (exist)
            {
                await _queueManagement.DeleteQueue(_workContext, testDefinition.QueueName);
            }

            // Act
            bool state = await new StateManagerBuilder()
                .Add(new CreateQueueState(_queueManagement, testDefinition))
                .Build()
                .Set(_workContext);

            state.Should().BeTrue();

            subjects = await _queueManagement.Search(_workContext);
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(1);
            subjects[0].QueueName.Should().BeEquivalentTo(testDefinition.QueueName);

            state = await new StateManagerBuilder()
                .Add(new RemoveQueueState(_queueManagement, testDefinition.QueueName))
                .Build()
                .Set(_workContext);

            state.Should().BeTrue();

            subjects = await _queueManagement.Search(_workContext);
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(0);
        }
    }
}
