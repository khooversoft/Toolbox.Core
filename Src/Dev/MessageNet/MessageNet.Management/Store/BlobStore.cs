// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using Khooversoft.Toolbox.Actor;
using Khooversoft.MessageHub.Interface;

namespace Khooversoft.MessageHub.Management
{
    public class BlobStore : IRegisterStore
    {
        private readonly IBlobRepository _blobRepository;
        private readonly Deferred _createContainer;

        public BlobStore(IBlobRepository blobRepository)
        {
            _blobRepository = blobRepository;
            _createContainer = new Deferred(x => _blobRepository.CreateContainer(x));
        }

        public Task Set(IWorkContext context, string path, NodeRegistrationModel nodeRegistrationModel)
        {
            _createContainer.Execute(context);

            string data = JsonConvert.SerializeObject(nodeRegistrationModel);
            return _blobRepository.Set(context, path, data);
        }

        public Task Remove(IWorkContext context, string path)
        {
            _createContainer.Execute(context);

            return _blobRepository.Delete(context, path);
        }

        public async Task<NodeRegistrationModel?> Get(IWorkContext context, string path)
        {
            _createContainer.Execute(context);

            string? data = await _blobRepository.Get(context, path);

            return data == null ? null : JsonConvert.DeserializeObject<NodeRegistrationModel>(data);
        }

        public async Task<IReadOnlyList<NodeRegistrationModel>> List(IWorkContext context, string search)
        {
            _createContainer.Execute(context);

            IReadOnlyList<string> list = await _blobRepository.List(context, search);

            NodeRegistrationModel[] result = await list
                .Select(x => Get(context, x))
                .Where(x => x != null)
                .WhenAll();

            return result;
        }

        public async Task ClearAll(IWorkContext context)
        {
            _createContainer.Execute(context);

            await _blobRepository.ClearAll(context);
        }
    }
}
