// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Property update types
    /// </summary>
    public enum PropertyUpdate
    {
        /// <summary>
        /// Fail when duplicate property is encountered
        /// </summary>
        FailOnDuplicate,

        /// <summary>
        /// Ignore duplicate, takes the source
        /// </summary>
        IgnoreDuplicate,

        /// <summary>
        /// Overwrite properties from source
        /// </summary>
        Overwrite,
    }
}
