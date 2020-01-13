using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class AssemblyLoader : IAssemblyLoader
    {
        public AssemblyLoader() { }

        public Assembly LoadFromAssemblyPath(string assemblyPathToLoad)
        {
            assemblyPathToLoad.Verify(nameof(assemblyPathToLoad)).IsNotEmpty();

            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPathToLoad);
        }
    }
}
