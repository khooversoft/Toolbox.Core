using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Pipeline block used to process messages, handles finalization of all blocks in the pipeline
    /// </summary>
    /// <typeparam name="T">message type</typeparam>
    public class PipelineBlock<T> : IPipelineBlock<T>
    {
        private const string _assertText = "Empty list";
        private readonly List<IDataflowBlock> _blockList = new List<IDataflowBlock>();
        private readonly List<IPipelineBlock<T>> _register = new List<IPipelineBlock<T>>();
        private readonly object _lock = new object();

        public PipelineBlock()
        {
        }

        /// <summary>
        /// Return number of block in the pipeline
        /// </summary>
        public int Count => _blockList.Count;

        /// <summary>
        /// Get current "Source Block", last added to the pipeline
        /// </summary>
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

        /// <summary>
        /// Get root block used to send message
        /// </summary>
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

        /// <summary>
        /// Get completion task
        /// </summary>
        public Task Completion
        {
            get
            {
                _blockList.Count.Verify().Assert(x => x > 0, _assertText);

                var tasks = _blockList.OfType<IDataflowBlock>()
                    .Select(x => x.Completion)
                    .Concat(_register.Select(x => x.Completion))
                    .ToArray();

                return Task.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Add dataflow block to pipeline
        /// </summary>
        /// <param name="source">block</param>
        /// <returns>this</returns>
        public IPipelineBlock<T> Add(IDataflowBlock source)
        {
            source.Verify(nameof(source)).IsNotNull();

            lock (_lock)
            {
                _blockList.Count.Verify().Assert(x => _blockList.Count > 1 || source is ITargetBlock<T>, "First block must be a target");
                _blockList.Add(source);
            }

            return this;
        }

        /// <summary>
        /// Add pipline block to pipeline
        /// </summary>
        /// <param name="source">pipeline</param>
        /// <returns>this</returns>
        public IPipelineBlock<T> Add(IPipelineBlock<T> source)
        {
            source.Verify(nameof(source)).IsNotNull();

            lock (_lock)
            {
                _register.Add(source);
            }

            return this;
        }

        /// <summary>
        /// Issue complete to pipeline
        /// </summary>
        public void Complete()
        {
            lock (_lock)
            {
                Root.Complete();
                _register.ForEach(x => x.Complete());
            }
        }

        /// <summary>
        /// Send message to pipeline
        /// </summary>
        /// <param name="value">message to send</param>
        /// <returns>true if sent</returns>
        public async Task<bool> Send(T value)
        {
            ITargetBlock<T> target = Root as ITargetBlock<T> ?? throw new InvalidOperationException("First block is not a target to send");
            return await target.SendAsync(value);
        }

        /// <summary>
        /// Send message to pipeline
        /// </summary>
        /// <param name="value">message to send</param>
        /// <returns>true if message is sent</returns>
        public bool Post(T value)
        {
            ITargetBlock<T> target = Root as ITargetBlock<T> ?? throw new InvalidOperationException("First block is not a target to send");
            return target.Post(value);
        }
    }
}
