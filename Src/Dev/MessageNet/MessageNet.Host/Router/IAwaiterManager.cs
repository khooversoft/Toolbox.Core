using Khooversoft.MessageNet.Interface;
using System;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal interface IAwaiterManager
    {
        AwaiterManager Add(Guid id, TaskCompletionSource<NetMessage> task);
        AwaiterManager SetResult(NetMessage? netMessage);
    }
}