using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartBlockServer
{
    internal class StreamRouter : IDisposable
    {
        private readonly IOption _option;
        private IPipelineBlock<RouteMessage> _pipelineBlock;
        private readonly IPipelineBlock<RouteMessage> _createBlockChainRoute;

        public StreamRouter(IOption option)
        {
            _option = option;

            _createBlockChainRoute = new PipelineBlock<RouteMessage>()
                .DoAction(x => CreateBlockChainRoute(x));

            _pipelineBlock = new PipelineBlock<RouteMessage>()
                .Register(_createBlockChainRoute)
                .Broadcast()
                .DoAction(x => _createBlockChainRoute.Post(x));
        }

        public void Dispose()
        {
            IPipelineBlock<RouteMessage>? block = Interlocked.Exchange(ref _pipelineBlock, null!);
            if( block != null)
            {
                block.CompleteAndWait().Wait();
            }
        }

        public Task Route(RouteMessage routeMessage)
        {
            _pipelineBlock.Post(routeMessage);
            return Task.CompletedTask;
        }

        public Task CreateBlockChainRoute(RouteMessage routeMessage)
        {
            Console.WriteLine("Made it");
            return Task.CompletedTask;
        }
    }
}
