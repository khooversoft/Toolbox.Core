using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public interface IPipelineBlock<T>
    {
        Task Completion { get; }
        ISourceBlock<T> Current { get; }
        IDataflowBlock Root { get; }
        int Count { get; }

        PipelineBlock<T> Add(IDataflowBlock source);
        void Complete();
        Task<bool> SendAsync(T value);
        bool Post(T value);
    }
}