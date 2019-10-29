namespace EventHubPerformanceTest
{
    public interface IOption
    {
        bool Help { get; }

        bool Send { get; }

        bool Receive { get; }

        int Count { get; }

        EventHub EventHub { get; }

        StorageAccount StorageAccount { get; }
        System.Threading.CancellationTokenSource CancellationTokenSource { get; }
    }
}