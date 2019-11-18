using System.Threading.Tasks;
using Microsoft.Azure.EventHubs.Processor;

namespace EventHubPerformanceTest
{
    internal interface IEventReceiverHost
    {
        Task RegisterEventProcessorFactoryAsync(IEventProcessorFactory factory, EventProcessorOptions processorOptions);

        Task UnregisterEventProcessorAsync();
    }
}