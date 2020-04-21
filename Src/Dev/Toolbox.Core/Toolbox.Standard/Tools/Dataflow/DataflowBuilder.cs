// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public class DataflowBuilder<T> : IDataflowCollection<T>, IEnumerable<IDataflow<T>>
    {
        private readonly IList<IDataflow<T>> _targets = new List<IDataflow<T>>();

        public DataflowBuilder() { }

        public void Add(IDataflow<T> targetBlock) => _targets.Add(targetBlock);

        public IDataflowSource<T> Build()
        {
            IList<IDataflowBlock> blockList = new List<IDataflowBlock>();

            var option = new DataflowLinkOptions
            {
                PropagateCompletion = true,
            };

            var stack = new Stack<IDataflow<T>>(_targets.Reverse());
            var rootStack = new Stack<ISourceBlock<T>>();

            while (stack.TryPop(out IDataflow<T> target))
            {
                switch (target)
                {
                    case PopMarker popMarker:
                        rootStack.Count.VerifyAssert<int, InvalidOperationException>(x => x > 0, _ => "Root stack is empty");
                        rootStack.Pop();
                        break;

                    case BroadcastDataflow<T> broadcast:
                        var broadcastBlock = new BroadcastBlock<T>(x => x);
                        blockList.Add(broadcastBlock);

                        if (rootStack.Count > 0) rootStack.Peek().LinkTo(broadcastBlock, option, x => broadcast.Predicate(x));
                        rootStack.Push(broadcastBlock);

                        stack.Push(new PopMarker());
                        broadcast.Reverse().ForEach(x => stack.Push(x));
                        break;

                    case SelectDataflow<T> select:
                        var transformBlock = new TransformBlock<T, T>(select.Transform);
                        blockList.Add(transformBlock);

                        if (rootStack.Count > 0) rootStack.Peek().LinkTo(transformBlock, option, x => select.Predicate(x));
                        rootStack.Push(transformBlock);
                        break;

                    case ActionDataflow<T> action:
                        if (rootStack.Count == 0)
                        {
                            var broadcast = new BroadcastBlock<T>(x => x);
                            rootStack.Push(broadcast);
                            blockList.Add(broadcast);
                        }

                        var actionBlock = new ActionBlock<T>(action.Action);
                        blockList.Add(actionBlock);
                        rootStack.Peek().LinkTo(actionBlock, option, x => action.Predicate(x));
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown target {target.GetType().Name}");
                }
            }

            return new DataflowSource<T>(blockList);
        }

        public IEnumerator<IDataflow<T>> GetEnumerator()
        {
            return _targets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _targets.GetEnumerator();
        }

        public static DataflowBuilder<T> operator +(DataflowBuilder<T> self, IDataflow<T> subject)
        {
            self.Add(subject);
            return self;
        }

        private class PopMarker : IDataflow<T>
        {
            public PopMarker()
            {
            }
        }
    }
}
