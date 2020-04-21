// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Toolbox.Standard
{
    public struct ModuleTypeRegistration
    {
        public ModuleTypeRegistration(Type objectType, Type interfaceType)
        {
            objectType.VerifyNotNull(nameof(objectType));
            interfaceType.VerifyNotNull(nameof(interfaceType));

            ObjectType = objectType;
            InterfaceType = interfaceType;
            InstancePerLifetimeScope = false;
        }

        public ModuleTypeRegistration(Type objectType, Type interfaceType, bool instancePerLifetimeScope)
            : this(objectType, interfaceType)
        {
            InstancePerLifetimeScope = instancePerLifetimeScope;
        }

        public Type ObjectType { get; }

        public Type InterfaceType { get; }

        public bool InstancePerLifetimeScope { get; }
    }
}
