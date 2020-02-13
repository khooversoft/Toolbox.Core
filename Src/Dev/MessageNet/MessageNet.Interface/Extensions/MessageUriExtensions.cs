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

        public static string QueueName(this Uri subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return subject.Host.ToEnumerable()
                .Concat(subject.LocalPath.Split('/'))
                .Do(x => string.Join("/", x));
        }
    }
}
