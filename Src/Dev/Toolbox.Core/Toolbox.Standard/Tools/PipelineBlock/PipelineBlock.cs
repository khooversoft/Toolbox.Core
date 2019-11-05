using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public class PipelineBlock<T> : IPipelineBlock<T>
    {
        private const string _assertText = "Empty list";
        private readonly List<IDataflowBlock> _blockList = new List<IDataflowBlock>();
        private readonly object _lock = new object();

        public PipelineBlock()
        {
        }

        public int Count => _blockList.Count;

        public ISourceBlock<T> Current
        {
            get
            {
                _blockList.Count.Verify().Assert(x => x > 0, _assertText);

                lock (_lock)
                {
                    return _blockList.OfType<ISourceBlock<T>>().Last();
                }
            }
        }

        public IDataflowBlock Root
        {
            get
            {
                _blockList.Count.Verify().Assert(x => x > 0, _assertText);

                lock (_lock)
                {
                    return _blockList[0];
                }
            }
        }

        public Task Completion
        {
            get
            {
                _blockList.Count.Verify().Assert(x => x > 0, _assertText);

                var tasks = _blockList.OfType<IDataflowBlock>()
                    .Select(x => x.Completion)
                    .ToArray();

                return Task.WhenAll(tasks);
            }
        }

        public PipelineBlock<T> Add(IDataflowBlock source)
        {
            source.Verify(nameof(source)).IsNotNull();

            lock (_lock)
            {
                _blockList.Count.Verify().Assert(x => _blockList.Count > 1 || source is ITargetBlock<T>, "First block must be a target");
                _blockList.Add(source);
            }

            return this;
        }

        public void Complete()
        {
            lock (_lock)
            {
                Root.Complete();
            }
        }

        public async Task<bool> SendAsync(T value)
        {
            ITargetBlock<T> target = Root as ITargetBlock<T> ?? throw new InvalidOperationException("First block is not a target to send");
            return await target.SendAsync(value);
        }

        public bool Post(T value)
        {
            ITargetBlock<T> target = Root as ITargetBlock<T> ?? throw new InvalidOperationException("First block is not a target to send");
            return target.Post(value);
        }
    }
}
