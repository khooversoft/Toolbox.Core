// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Azure
{
    public class QueueDefinition
    {
        public QueueDefinition(string queueName)
        {
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            QueueName = queueName;
        }

        public string QueueName { get; }

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
            return obj switch
            {
                QueueDefinition compareTo => QueueName == compareTo.QueueName &&
                    LockDuration == compareTo.LockDuration &&
                    RequiresDuplicateDetection == compareTo.RequiresDuplicateDetection &&
                    DuplicateDetectionHistoryTimeWindow == compareTo.DuplicateDetectionHistoryTimeWindow &&
                    RequiresSession == compareTo.RequiresSession &&
                    DefaultMessageTimeToLive == compareTo.DefaultMessageTimeToLive &&
                    AutoDeleteOnIdle == compareTo.AutoDeleteOnIdle &&
                    EnableDeadLetteringOnMessageExpiration == compareTo.EnableDeadLetteringOnMessageExpiration &&
                    MaxDeliveryCount == compareTo.MaxDeliveryCount &&
                    EnablePartitioning == compareTo.EnablePartitioning,

                _ => false,
            };
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(QueueDefinition left, QueueDefinition right) => left.Equals(right);

        public static bool operator !=(QueueDefinition left, QueueDefinition right) => !left.Equals(right);
    }
}
