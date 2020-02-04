namespace Khooversoft.Toolbox.Standard
{
    public interface IDataflowCollection<T>
    {
        void Add(IDataflow<T> targetBlock);
    }
}