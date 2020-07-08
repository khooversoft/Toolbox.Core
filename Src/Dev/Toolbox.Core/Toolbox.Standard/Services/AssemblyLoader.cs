// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Load assembly into the default context
    /// </summary>
    public class AssemblyLoader : IAssemblyLoader
    {
        public AssemblyLoader() { }

        public Assembly LoadFromAssemblyPath(string assemblyPathToLoad)
        {
            assemblyPathToLoad.VerifyNotEmpty(nameof(assemblyPathToLoad));

            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPathToLoad);
        }
    }
}
