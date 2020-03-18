using FluentAssertions;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ToolBox.Azure.Test.Queue
{
    [Collection("QueueTests")]
    public class QueueManagementTests : IClassFixture<ApplicationFixture>
    {
        private readonly IWorkContext _workContext = WorkContextBuilder.Default;
        private readonly QueueManagement _queueManagement;
        private readonly QueueDefinition _queueDefinition = new QueueDefinition("Unit1_TestQueue1");

        public QueueManagementTests(ApplicationFixture application)
        {
            _queueManagement = new QueueManagement(application.ConnectionString);
        }

        [Fact(Skip = "IntegrationTests")]
        public async Task GivenQueueDoesNotExist_WhenDelete_ShouldNotRaiseException()
        {
            // Verify queue does not exist, if so delete it.
            bool exist = await _queueManagement.QueueExists(_workContext, _queueDefinition.QueueName!);
            if (exist)
            {
                await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);
            }

            Func<Task> act = async () => await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);

            await act.Should().ThrowAsync<MessagingEntityNotFoundException>();
        }

        [Fact(Skip = "IntegrationTests")]
        public async Task GivenQueueDoesNotExist_WhenSearched_ShouldReturnEmptyList()
        {
            // Verify queue does not exist, if so delete it.
            bool exist = await _queueManagement.QueueExists(_workContext, _queueDefinition.QueueName!);
            if (exist)
            {
                await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);
            }

            IReadOnlyList<QueueDefinition> subjects = await _queueManagement.Search(_workContext, _queueDefinition.QueueName);
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(0);
        }

        [Fact(Skip = "IntegrationTests")]
        public async Task GivenServiceQueue_WhenGetAndNotExist_ShouldThrow()
        {
            // Verify queue does not exist, if so delete it.
            bool exist = await _queueManagement.QueueExists(_workContext, _queueDefinition.QueueName!);
            if (exist)
            {
                await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);
            }

            Func<Task> act = async () => await _queueManagement.GetQueue(_workContext, _queueDefinition.QueueName!);

            await act.Should().ThrowAsync<MessagingEntityNotFoundException>();
        }

        [Fact(Skip = "IntegrationTests")]
        public async Task GivenServiceQueue_WhenGetAndExist_ReturnData()
        {
            // Verify queue does not exist, if so delete it.
            bool exist = await _queueManagement.QueueExists(_workContext, _queueDefinition.QueueName!);
            if (exist)
            {
                await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);
            }

            // Create queue
            QueueDefinition subject = await _queueManagement.CreateQueue(_workContext, _queueDefinition);
            subject.Should().NotBeNull();
            (_queueDefinition == subject).Should().BeTrue();

            subject = await _queueManagement.GetQueue(_workContext, _queueDefinition.QueueName!);
            subject.Should().NotBeNull();
            (_queueDefinition == subject).Should().BeTrue();
        }


        [Fact(Skip = "IntegrationTests")]
        public async Task GivenServiceQueue_WhenCreatedTwice_ShouldThrow()
        {
            // Verify queue does not exist, if so delete it.
            bool exist = await _queueManagement.QueueExists(_workContext, _queueDefinition.QueueName!);
            if (exist)
            {
                await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);
            }

            // Create queue
            QueueDefinition subject = await _queueManagement.CreateQueue(_workContext, _queueDefinition);
            subject.Should().NotBeNull();
            (_queueDefinition == subject).Should().BeTrue();

            Func<Task> act = async () => await _queueManagement.CreateQueue(_workContext, _queueDefinition);

            await act.Should().ThrowAsync<MessagingEntityAlreadyExistsException>();

            await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);
        }

        [Fact(Skip = "IntegrationTests")]
        public async Task GivenServiceExistQueue_WhenSearchedWithoutWildcard_ShouldFind()
        {
            // Verify queue does not exist, if so delete it.
            bool exist = await _queueManagement.QueueExists(_workContext, _queueDefinition.QueueName!);
            if (!exist)
            {
                QueueDefinition createSubject = await _queueManagement.CreateQueue(_workContext, _queueDefinition);
                createSubject.Should().NotBeNull();
                (_queueDefinition == createSubject).Should().BeTrue();
            }

            IReadOnlyList<QueueDefinition> subjects = await _queueManagement.Search(_workContext, _queueDefinition.QueueName);
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(1);
            subjects[0].QueueName.Should().BeEquivalentTo(_queueDefinition.QueueName);

            await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);
        }

        [Fact(Skip = "IntegrationTests")]
        public async Task GivenServiceExistQueue_WhenSearchedWildcard_ShouldFind()
        {
            // Verify queue does not exist, if so delete it.
            bool exist = await _queueManagement.QueueExists(_workContext, _queueDefinition.QueueName!);
            if (!exist)
            {
                QueueDefinition createSubject = await _queueManagement.CreateQueue(_workContext, _queueDefinition);
                createSubject.Should().NotBeNull();
                (_queueDefinition == createSubject).Should().BeTrue();
            }

            IReadOnlyList<QueueDefinition> subjects = await _queueManagement.Search(_workContext, "unit1*");
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(1);
            subjects[0].QueueName.Should().BeEquivalentTo(_queueDefinition.QueueName);

            await _queueManagement.DeleteQueue(_workContext, _queueDefinition.QueueName!);
        }
    }
}
