// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.MessageNet.Client
{
    public interface IMessageClient
    {
        Task Close();

        void Dispose();

        Task Send(IWorkContext context, string message);
    }
}