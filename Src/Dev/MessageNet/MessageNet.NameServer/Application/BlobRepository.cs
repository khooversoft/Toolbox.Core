// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;

namespace MessageHub.NameServer
{
    public class BlobRepositoryDetails
    {
        public string ContainerName { get; set; } = null!;

        public string Connection { get; set; } = null!;

        public void Verify()
        {
            ContainerName!.Verify(nameof(ContainerName)).IsNotEmpty();
            Connection!.Verify(nameof(Connection)).IsNotEmpty();
        }
    }
}
