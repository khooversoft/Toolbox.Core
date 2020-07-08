// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Interface for the assembly loader services
    /// </summary>
    public interface IAssemblyLoader
    {
        Assembly LoadFromAssemblyPath(string assemblyPathToLoad);
    }
}