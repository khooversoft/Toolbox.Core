// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using Autofac;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.Toolbox.Autofac
{
    public static class AutofacExtensions
    {
        public static void RegisterContainerModule(this ContainerBuilder containerBuilder, ContainerRegistrationModule containerRegistrationModule)
        {
            containerBuilder.Verify(nameof(containerBuilder)).IsNotNull();
            containerRegistrationModule.Verify(nameof(containerRegistrationModule)).IsNotNull();

            foreach(var item in containerRegistrationModule)
            {
                var regType = containerBuilder.RegisterType(item.ObjectType).As(item.InterfaceType);
                if( item.InstancePerLifetimeScope)
                {
                    regType.InstancePerLifetimeScope();
                }
            }
        }
    }
}
