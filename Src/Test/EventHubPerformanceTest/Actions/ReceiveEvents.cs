using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    public class ReceiveEvents
    {
        private readonly IOption _option;

        public ReceiveEvents(IOption option)
        {
            _option = option;
        }

        public async Task Run()
        {
            var eventProcessorHost = new EventProcessorHost(
                eventHubPath: _option.EventHub.Name,
                consumerGroupName: PartitionReceiver.DefaultConsumerGroupName,
                eventHubConnectionString: _option.EventHub.ConnectionString,
                storageConnectionString: _option.StorageAccount.ConnectionString,
                leaseContainerName: _option.StorageAccount.ContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<EventProcessor>();

            while(!_option.CancellationTokenSource.Token.IsCancellationRequested)
            {
                await Task.Delay(500);
            }

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }

        private class EventProcessor : IEventProcessor
        {
            public Task CloseAsync(PartitionContext context, CloseReason reason)
            {
                Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
                return Task.CompletedTask;
            }

            public Task OpenAsync(PartitionContext context)
            {
                Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
                return Task.CompletedTask;
            }

            public Task ProcessErrorAsync(PartitionContext context, Exception error)
            {
                Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
                return Task.CompletedTask;
            }

            public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
            {
                foreach (var eventData in messages)
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");
                }

                return context.CheckpointAsync();
            }
        }
    }
}
