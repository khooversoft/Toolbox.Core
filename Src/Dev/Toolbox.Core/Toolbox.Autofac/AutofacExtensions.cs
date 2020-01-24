// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (var item in containerRegistrationModule)
            {
                var regType = containerBuilder.RegisterType(item.ObjectType).As(item.InterfaceType);
                if (item.InstancePerLifetimeScope)
                {
                    regType.InstancePerLifetimeScope();
                }
            }
        }

        public static ServiceContainerBuilder SetLifetimeScope(this ServiceContainerBuilder serviceContainer, ILifetimeScope lifetimeScope)
        {
            serviceContainer
                .SetService(x => lifetimeScope.Resolve(x))
                .SetServiceOptional(x => lifetimeScope.ResolveOptional(x))
                .SetCreateScope((tag, types) => lifetimeScope.BeginLifetimeScope(tag, x => types.ForEach(x => lifetimeScope.Resolve(x))));

            return serviceContainer;
        }

        public static ILifetimeScope BeginLifetimeScope(this IServiceContainer serviceContainer, string tag) =>
            (ILifetimeScope)serviceContainer.CreateScope(tag, Enumerable.Empty<Type>());

        public static ILifetimeScope BeginLifetimeScope(this IServiceContainer serviceContainer, string tag, IEnumerable<Type> types) =>
            (ILifetimeScope)serviceContainer.CreateScope(tag, types);
    }
}
