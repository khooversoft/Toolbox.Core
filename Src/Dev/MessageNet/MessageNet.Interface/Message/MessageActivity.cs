using System;
using System.Collections.Generic;
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
            if (obj is MessageActivity header)
            {
                return ActivityId == header.ActivityId &&
                    ParentActivityId == header.ParentActivityId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ActivityId, ParentActivityId);
        }

        public static bool operator ==(MessageActivity v1, MessageActivity v2) => v1?.Equals(v2) ?? false;

        public static bool operator !=(MessageActivity v1, MessageActivity v2) => !v1?.Equals(v2) ?? false;
    }
}
