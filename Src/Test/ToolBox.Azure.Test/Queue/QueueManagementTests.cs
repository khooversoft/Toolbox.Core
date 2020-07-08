using FluentAssertions;
using Khoover.Toolbox.TestTools;
using Khooversoft.Toolbox.Azure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ToolBox.Azure.Test.Queue
{
    [Collection("QueueTests")]
    public class QueueManagementTests
    {
        private readonly QueueDefinition _queueDefinition = new QueueDefinition("Unit1_TestQueue1");
        private readonly ILoggerFactory _testLoggerFactory = new TestLoggerFactory();
        private readonly AzureTestOption _testOption;

        public QueueManagementTests()
        {
            _testOption = new TestOptionBuilder().Build();
        }

        //[Fact(Skip = "IntegrationTests")]
        [Fact]
        public async Task GivenQueueDoesNotExist_WhenDelete_ShouldNotRaiseException()
        {
            IQueueManagement queue = _testOption.GetQueueManagement(_testLoggerFactory);

            // Verify queue does not exist, if so delete it.
            bool exist = await queue.Exist(_queueDefinition.QueueName!, CancellationToken.None);
            if (exist)
            {
                await queue.Delete(_queueDefinition.QueueName!, CancellationToken.None);
            }

            Func<Task> act = async () => await queue.Delete(_queueDefinition.QueueName!, CancellationToken.None);

            await act.Should().ThrowAsync<MessagingEntityNotFoundException>();
        }

        //[Fact(Skip = "IntegrationTests")]
        [Fact]
        public async Task GivenQueueDoesNotExist_WhenSearched_ShouldReturnEmptyList()
        {
            IQueueManagement queue = _testOption.GetQueueManagement(_testLoggerFactory);

            // Verify queue does not exist, if so delete it.
            bool exist = await queue.Exist(_queueDefinition.QueueName!, CancellationToken.None);
            if (exist)
            {
                await queue.Delete(_queueDefinition.QueueName!, CancellationToken.None);
            }

            IReadOnlyList<QueueDefinition> subjects = await queue.Search(CancellationToken.None, _queueDefinition.QueueName);
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(0);
        }

        //[Fact(Skip = "IntegrationTests")]
        [Fact]
        public async Task GivenServiceQueue_WhenGetAndNotExist_ShouldThrow()
        {
            IQueueManagement queue = _testOption.GetQueueManagement(_testLoggerFactory);

            // Verify queue does not exist, if so delete it.
            bool exist = await queue.Exist(_queueDefinition.QueueName!, CancellationToken.None);
            if (exist)
            {
                await queue.Delete(_queueDefinition.QueueName!, CancellationToken.None);
            }

            Func<Task> act = async () => await queue.GetDefinition(_queueDefinition.QueueName!, CancellationToken.None);

            await act.Should().ThrowAsync<MessagingEntityNotFoundException>();
        }

        //[Fact(Skip = "IntegrationTests")]
        [Fact]
        public async Task GivenServiceQueue_WhenGetAndExist_ReturnData()
        {
            IQueueManagement queue = _testOption.GetQueueManagement(_testLoggerFactory);

            // Verify queue does not exist, if so delete it.
            bool exist = await queue.Exist(_queueDefinition.QueueName!, CancellationToken.None);
            if (exist)
            {
                await queue.Delete(_queueDefinition.QueueName!, CancellationToken.None);
            }

            // Create queue
            QueueDefinition subject = await queue.Create(_queueDefinition, CancellationToken.None);
            subject.Should().NotBeNull();
            (_queueDefinition == subject).Should().BeTrue();

            subject = await queue.GetDefinition(_queueDefinition.QueueName!, CancellationToken.None);
            subject.Should().NotBeNull();
            (_queueDefinition == subject).Should().BeTrue();
        }


        //[Fact(Skip = "IntegrationTests")]
        [Fact]
        public async Task GivenServiceQueue_WhenCreatedTwice_ShouldThrow()
        {
            IQueueManagement queue = _testOption.GetQueueManagement(_testLoggerFactory);

            // Verify queue does not exist, if so delete it.
            bool exist = await queue.Exist(_queueDefinition.QueueName!, CancellationToken.None);
            if (exist)
            {
                await queue.Delete(_queueDefinition.QueueName!, CancellationToken.None);
            }

            // Create queue
            QueueDefinition subject = await queue.Create(_queueDefinition, CancellationToken.None);
            subject.Should().NotBeNull();
            (_queueDefinition == subject).Should().BeTrue();

            Func<Task> act = async () => await queue.Create(_queueDefinition, CancellationToken.None);

            await act.Should().ThrowAsync<MessagingEntityAlreadyExistsException>();

            await queue.Delete(_queueDefinition.QueueName!, CancellationToken.None);
        }

        //[Fact(Skip = "IntegrationTests")]
        [Fact]
        public async Task GivenServiceExistQueue_WhenSearchedWithoutWildcard_ShouldFind()
        {
            IQueueManagement queue = _testOption.GetQueueManagement(_testLoggerFactory);

            // Verify queue does not exist, if so delete it.
            bool exist = await queue.Exist(_queueDefinition.QueueName!, CancellationToken.None);
            if (!exist)
            {
                QueueDefinition createSubject = await queue.Create(_queueDefinition, CancellationToken.None);
                createSubject.Should().NotBeNull();
                (_queueDefinition == createSubject).Should().BeTrue();
            }

            IReadOnlyList<QueueDefinition> subjects = await queue.Search(CancellationToken.None, _queueDefinition.QueueName);
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(1);
            subjects[0].QueueName.Should().BeEquivalentTo(_queueDefinition.QueueName);

            await queue.Delete(_queueDefinition.QueueName!, CancellationToken.None);
        }

        //[Fact(Skip = "IntegrationTests")]
        [Fact]
        public async Task GivenServiceExistQueue_WhenSearchedWildcard_ShouldFind()
        {
            IQueueManagement queue = _testOption.GetQueueManagement(_testLoggerFactory);

            // Verify queue does not exist, if so delete it.
            bool exist = await queue.Exist(_queueDefinition.QueueName!, CancellationToken.None);
            if (!exist)
            {
                QueueDefinition createSubject = await queue.Create(_queueDefinition, CancellationToken.None);
                createSubject.Should().NotBeNull();
                (_queueDefinition == createSubject).Should().BeTrue();
            }

            IReadOnlyList<QueueDefinition> subjects = await queue.Search(CancellationToken.None, "unit1*");
            subjects.Should().NotBeNull();
            subjects.Count.Should().Be(1);
            subjects[0].QueueName.Should().BeEquivalentTo(_queueDefinition.QueueName);

            await queue.Delete(_queueDefinition.QueueName!, CancellationToken.None);
        }
    }
}
