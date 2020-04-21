// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Actor registration for lambda activator
    /// </summary>
    public class ActorTypeRegistration
    {
        public ActorTypeRegistration(Type interfaceType, Func<IWorkContext, IActor> createImplementation)
        {
            interfaceType.VerifyNotNull(nameof(interfaceType));
            createImplementation.VerifyNotNull(nameof(createImplementation));

            InterfaceType = interfaceType;
            CreateImplementation = createImplementation;
        }

        /// <summary>
        /// Interface type
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// Create implementation by lambda
        /// </summary>
        public Func<IWorkContext, IActor> CreateImplementation { get; }
    }
}
