// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Management
{
    public static class QueueDefinitionExtensions
    {
        public static QueueDescription ConvertTo(this QueueDefinition subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();
            subject.QueueName!.Verify(nameof(subject.QueueName)).IsNotEmpty();

            return new QueueDescription(subject.QueueName)
            {
                LockDuration = subject.LockDuration,
                RequiresDuplicateDetection = subject.RequiresDuplicateDetection,
                DuplicateDetectionHistoryTimeWindow = subject.DuplicateDetectionHistoryTimeWindow,
                RequiresSession = subject.RequiresSession,
                DefaultMessageTimeToLive = subject.DefaultMessageTimeToLive,
                AutoDeleteOnIdle = subject.AutoDeleteOnIdle,
                EnableDeadLetteringOnMessageExpiration = subject.EnableDeadLetteringOnMessageExpiration,
                MaxDeliveryCount = subject.MaxDeliveryCount,
                EnablePartitioning = subject.EnablePartitioning,
            };
        }

        public static QueueDefinition ConvertTo(this QueueDescription subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new QueueDefinition
            {
                QueueName = subject.Path,
                LockDuration = subject.LockDuration,
                RequiresDuplicateDetection = subject.RequiresDuplicateDetection,
                DuplicateDetectionHistoryTimeWindow = subject.DuplicateDetectionHistoryTimeWindow,
                RequiresSession = subject.RequiresSession,
                DefaultMessageTimeToLive = subject.DefaultMessageTimeToLive,
                AutoDeleteOnIdle = subject.AutoDeleteOnIdle,
                EnableDeadLetteringOnMessageExpiration = subject.EnableDeadLetteringOnMessageExpiration,
                MaxDeliveryCount = subject.MaxDeliveryCount,
                EnablePartitioning = subject.EnablePartitioning,
            };
        }
    }
}
