// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Management
{
    /// <summary>
    /// Verify queue does exist or create it
    /// </summary>
    public class CreateBlobContainerState : IStateItem
    {
        private readonly BlobStoreConnection _blobStoreConnection;
        private readonly IBlobRepository _blobRepository;

        public CreateBlobContainerState(IBlobRepository blobRepository, BlobStoreConnection blobStoreConnection)
        {
            blobRepository.Verify(nameof(blobRepository)).IsNotNull();
            blobStoreConnection.Verify(nameof(blobStoreConnection)).IsNotNull();
            blobStoreConnection.ContainerName.Verify(nameof(blobStoreConnection.ContainerName)).IsNotNull();

            _blobStoreConnection = blobStoreConnection;
            _blobRepository = blobRepository;
        }

        public string Name => _blobStoreConnection.ContainerName;

        public bool IgnoreError => false;

        public async Task<bool> Set(IWorkContext context)
        {
            await _blobRepository.CreateContainer(context);
            return true;
        }

        public async Task<bool> Test(IWorkContext context)
        {
            await _blobRepository.CreateContainer(context);
            return true;
        }
    }
}
