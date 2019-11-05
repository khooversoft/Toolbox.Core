//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks.Dataflow;

//namespace Khooversoft.Toolbox.Standard
//{
//    public static class BlockExtensions
//    {
//        public static ISourceBlock<T> RouteTo<T>(this ISourceBlock<T> block, Action<T> action, Predicate<T>? predicate = null)
//        {
//            var actionBlock = new ActionBlock<T>(action);
//            block.SendTo(actionBlock, predicate);

//            return block;
//        }

//        public static ISourceBlock<T> RouteTo<T>(this ISourceBlock<T> block, Action<T> action, out IDisposable disposable, Predicate<T>? predicate = null)
//        {
//            var actionBlock = new ActionBlock<T>(action);
//            disposable = block.SendTo(actionBlock, predicate);

//            return block;
//        }

//        public static ISourceBlock<TResult> Select<TSource, TResult>(this ISourceBlock<TSource> block, Func<TSource, TResult> action, Predicate<TSource>? predicate = null)
//        {
//            var transformBlock = new TransformBlock<TSource, TResult>(action);
//            block.SendTo(transformBlock, predicate);

//            return transformBlock;
//        }

//        public static ISourceBlock<T> Broadcast<T>(this ISourceBlock<T> block, Predicate<T>? predicate = null)
//        {
//            var broadcastBlock = new BroadcastBlock<T>(x => x);
//            block.SendTo(broadcastBlock, predicate);

//            return broadcastBlock;
//        }

//        public static ISourceBlock<T> Buffer<T>(this ISourceBlock<T> block, Predicate<T>? predicate = null)
//        {
//            var buffer = new BufferBlock<T>();
//            block.SendTo(buffer, predicate);

//            return buffer;
//        }

//        public static IDisposable SendTo<T>(this ISourceBlock<T> source, ITargetBlock<T> target, Predicate<T>? predicate = null)
//        {
//            var option = new DataflowLinkOptions
//            {
//                PropagateCompletion = true,
//            };

//            if (predicate != null)
//            {
//                return source.LinkTo(target, option, predicate);
//            }
//            else
//            {
//                return source.LinkTo(target, option);
//            }
//        }
//    }
//}
