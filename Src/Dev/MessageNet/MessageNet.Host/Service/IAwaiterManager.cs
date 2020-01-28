using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal interface IAwaiterManager
    {
        AwaiterManager Add(string id, TaskCompletionSource<bool> task);
        AwaiterManager SetResult(string id, bool state);
    }
}