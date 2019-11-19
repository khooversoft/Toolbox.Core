using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public class QueueDefinition
    {
        public string? QueueName { get; set; }

        public TimeSpan LockDuration { get; set; } = TimeSpan.FromSeconds(45);

        public bool RequiresDuplicateDetection { get; set; } = false;

        public TimeSpan DuplicateDetectionHistoryTimeWindow { get; set; } = TimeSpan.FromMinutes(10);

        public bool RequiresSession { get; set; } = false;

        public TimeSpan DefaultMessageTimeToLive { get; set; } = TimeSpan.FromDays(7);

        public TimeSpan AutoDeleteOnIdle { get; set; } = TimeSpan.MaxValue;

        public bool EnableDeadLetteringOnMessageExpiration { get; set; } = false;

        public int MaxDeliveryCount { get; set; } = 8;

        public bool EnablePartitioning { get; set; } = false;

        public override bool Equals(object obj)
        {
            QueueDefinition? compareTo = obj as QueueDefinition;

            return obj != null &&
                compareTo! != null! &&
                QueueName == compareTo?.QueueName &&
                LockDuration == compareTo!.LockDuration &&
                RequiresDuplicateDetection == compareTo.RequiresDuplicateDetection &&
                DuplicateDetectionHistoryTimeWindow == compareTo.DuplicateDetectionHistoryTimeWindow &&
                RequiresSession == compareTo.RequiresSession &&
                DefaultMessageTimeToLive == compareTo.DefaultMessageTimeToLive &&
                AutoDeleteOnIdle == compareTo.AutoDeleteOnIdle &&
                EnableDeadLetteringOnMessageExpiration == compareTo.EnableDeadLetteringOnMessageExpiration &&
                MaxDeliveryCount == compareTo.MaxDeliveryCount &&
                EnablePartitioning == compareTo.EnablePartitioning;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(QueueDefinition left, QueueDefinition right) => left.Equals(right);

        public static bool operator !=(QueueDefinition left, QueueDefinition right) => !left.Equals(right);
    }
}
