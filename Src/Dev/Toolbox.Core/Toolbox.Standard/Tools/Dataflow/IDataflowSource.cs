using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public interface IDataflowSource<T>
    {
        Task<bool> PostAsync(T message);

        bool Post(T message);

        void Complete();

        Task Completion { get; }
    }
}
