// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class MessageUriExtensions
    {
        public static MessageUri ToMessageUri(this string subject) => MessageUriBuilder.Parse(subject).Build();

        public static MessageUri ToMessageUri(this QueueId subject)
        {
            subject.VerifyNotNull(nameof(subject));

            return new MessageUri(subject.Namespace, subject.NetworkId, subject.NodeId);
        }

        public static QueueId ToQueueId(this MessageUri subject) => subject.VerifyNotNull(nameof(subject))
            .Func(x => new QueueId(x.Namespace, x.NetworkId, x.NodeId));
    }
}
