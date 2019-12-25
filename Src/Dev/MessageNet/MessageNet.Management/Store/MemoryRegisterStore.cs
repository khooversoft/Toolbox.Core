// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageHub.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageHub.Management
{
    public class MemoryRegisterStore : IRegisterStore
    {
        private readonly ConcurrentDictionary<string, NodeRegistrationModel> _data = new ConcurrentDictionary<string, NodeRegistrationModel>(StringComparer.OrdinalIgnoreCase);

        public MemoryRegisterStore()
        {
        }

        public Task ClearAll(IWorkContext context)
        {
            _data.Clear();
            return Task.CompletedTask;
        }

        public Task<NodeRegistrationModel?> Get(IWorkContext context, string path)
        {
            if (_data.TryGetValue(path, out NodeRegistrationModel model))
            {
                return Task.FromResult<NodeRegistrationModel?>(model);
            }

            return Task.FromResult<NodeRegistrationModel?>(null);
        }

        public Task<IReadOnlyList<NodeRegistrationModel>> List(IWorkContext context, string search)
        {
            var nodeIdCompare = new NodeIdCompare(search);

            var results = _data.Values
                .Where(x => nodeIdCompare.Test(x.NodeId!))
                .ToList();

            return Task.FromResult<IReadOnlyList<NodeRegistrationModel>>(results);
        }

        public Task Remove(IWorkContext context, string path)
        {
            _data.Remove(path, out NodeRegistrationModel _);
            return Task.FromResult(0);
        }

        public Task Set(IWorkContext context, string path, NodeRegistrationModel nodeRegistrationModel)
        {
            nodeRegistrationModel.Verify(nameof(nodeRegistrationModel)).IsNotNull();
            nodeRegistrationModel.NodeId!.Verify(nameof(nodeRegistrationModel.NodeId)).IsNotEmpty();

            _data.AddOrUpdate(nodeRegistrationModel.NodeId!, nodeRegistrationModel, (k, v) => nodeRegistrationModel);
            return Task.CompletedTask;
        }
    }
}
