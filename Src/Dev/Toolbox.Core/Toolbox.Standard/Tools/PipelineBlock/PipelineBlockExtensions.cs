// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public static class PipelineBlockExtensions
    {
        public static IPipelineBlock<T> DoAction<T>(this IPipelineBlock<T> block, Action<T> action, Predicate<T>? predicate = null)
        {
            var actionBlock = new ActionBlock<T>(action);

            if (block.Count > 0)
            {
                block.Current.LinkToWithPredicate(actionBlock, predicate);
            }

            block.Add(actionBlock);
            return block;
        }

        public static IPipelineBlock<T> DoAction<T>(this IPipelineBlock<T> block, Action<T> action, out IDisposable? disposable, Predicate<T>? predicate = null)
        {
            var actionBlock = new ActionBlock<T>(action);

            disposable = default;
            if (block.Count > 0)
            {
                disposable = block.Current.LinkToWithPredicate(actionBlock, predicate);
            }

            block.Add(actionBlock);
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

        public static IPipelineBlock<T> Register<T>(this IPipelineBlock<T> block, IPipelineBlock<T> blockToRegister)
        {
            block.Add(blockToRegister);
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
