// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class MessageActivity : INetMessageItem
    {
        public MessageActivity(Guid activityId, Guid? parentActivityId = null)
        {
            ActivityId = activityId;
            ParentActivityId = parentActivityId;
        }

        public Guid ActivityId { get; }

        public Guid? ParentActivityId { get; }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                MessageActivity header => ActivityId == header.ActivityId &&
                    ParentActivityId == header.ParentActivityId,

                _ => false,
            };
        }

        public override int GetHashCode() => HashCode.Combine(ActivityId, ParentActivityId);

        public static bool operator ==(MessageActivity v1, MessageActivity v2) => v1?.Equals(v2) ?? false;

        public static bool operator !=(MessageActivity v1, MessageActivity v2) => !v1?.Equals(v2) ?? false;
    }
}
