// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using Khooversoft.MessageHub.Management;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Khooversoft.MessageHub.Interface;

namespace Khooversoft.MessageHub.Management
{
    public class NodeRegistrationManagementActor : ActorBase, INodeRegistrationManagementActor
    {
        private readonly IRegisterStore _registerStore;

        public NodeRegistrationManagementActor(IRegisterStore registerStore)
        {
            registerStore.Verify(nameof(registerStore)).IsNotNull();

            _registerStore = registerStore;
        }

        public Task<IReadOnlyList<NodeRegistrationModel>> List(IWorkContext context, string search)
        {
            return _registerStore.List(context, search);
        }

        public async Task ClearRegistery(IWorkContext context)
        {
            await ActorManager.DeactivateAll();
            await _registerStore.ClearAll(context);
        }
    }
}
