using System.Reflection;

namespace Khooversoft.Toolbox.Standard
{
    public interface IAssemblyLoader
    {
        Assembly LoadFromAssemblyPath(string assemblyPathToLoad);
    }
}