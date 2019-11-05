using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public static class PipelineBlockExtensions
    {
        public static IPipelineBlock<T> RouteTo<T>(this IPipelineBlock<T> block, Action<T> action, Predicate<T>? predicate = null)
        {
            block.Count.Verify().Assert(x => x > 0, "No block sources registered");

            var actionBlock = new ActionBlock<T>(action);
            block.Add(actionBlock);
            block.Current.LinkToWithPredicate(actionBlock, predicate);

            return block;
        }

        public static IPipelineBlock<T> RouteTo<T>(this IPipelineBlock<T> block, Action<T> action, out IDisposable disposable, Predicate<T>? predicate = null)
        {
            block.Count.Verify().Assert(x => x > 0, "No block sources registered");

            var actionBlock = new ActionBlock<T>(action);
            block.Add(actionBlock);
            disposable = block.Current.LinkToWithPredicate(actionBlock, predicate);

            return block;
        }

        public static IPipelineBlock<T> Select<T>(this IPipelineBlock<T> block, Func<T, T> action, Predicate<T>? predicate = null)
        {
            var transformBlock = new TransformBlock<T, T>(action);

            if (block.Count > 0)
            {
                block.Current.LinkToWithPredicate(transformBlock, predicate);
            }

            block.Add(transformBlock);
            return block;
        }

        public static IPipelineBlock<T> Broadcast<T>(this IPipelineBlock<T> block, Predicate<T>? predicate = null)
        {
            var broadcastBlock = new BroadcastBlock<T>(x => x);

            if (block.Count > 0)
            {
                block.Current.LinkToWithPredicate(broadcastBlock, predicate);
            }

            block.Add(broadcastBlock);
            return block;
        }

        private static IDisposable LinkToWithPredicate<T>(this ISourceBlock<T> source, ITargetBlock<T> target, Predicate<T>? predicate = null)
        {
            var option = new DataflowLinkOptions
            {
                PropagateCompletion = true,
            };

            if (predicate != null)
            {
                return source.LinkTo(target, option, predicate);
            }
            else
            {
                return source.LinkTo(target, option);
            }
        }
    }
}
