// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class ContainerRegistrationModule : IEnumerable<ModuleTypeRegistration>
    {
        private readonly List<ModuleTypeRegistration> _list;

        public ContainerRegistrationModule()
        {
            _list = new List<ModuleTypeRegistration>();
        }

        public ContainerRegistrationModule(IEnumerable<ModuleTypeRegistration> registratons)
        {
            _list = new List<ModuleTypeRegistration>(registratons);
        }

        public ContainerRegistrationModule Add(Type objectType, Type interfaceType, bool instancePerLifetimeScope = false)
        {
            objectType.Verify(nameof(objectType)).IsNotNull();
            interfaceType.Verify(nameof(interfaceType)).IsNotNull();

            _list.Add(new ModuleTypeRegistration(objectType, interfaceType, instancePerLifetimeScope));
            return this;
        }

        public ContainerRegistrationModule Add(params ModuleTypeRegistration[] keyValuePairs)
        {
            keyValuePairs.Verify(nameof(keyValuePairs)).IsNotNull();

            _list.AddRange(keyValuePairs);
            return this;
        }

        public IEnumerator<ModuleTypeRegistration> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
