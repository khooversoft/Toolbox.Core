// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

namespace Khooversoft.MessageNet.Host
{
    public interface IConnectionManager
    {
        ConnectionManager Add(params ConnectionRegistration[] connectionRegistrations);

        string GetConnection(string networkId);
    }
}