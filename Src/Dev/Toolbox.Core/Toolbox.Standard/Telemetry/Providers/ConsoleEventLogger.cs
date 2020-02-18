// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class ConsoleEventLogger : ITelemetryLogger
    {
        private object _lock = new object();

        public ConsoleEventLogger()
        {
        }

        public void Write(TelemetryMessage message)
        {
            var list = new string?[]
            {
                message.EventDate.ToString("yyMMdd:HH:mm:ss") + (message.EventDate.Offset.Hours >= 0 ? "+" : string.Empty) + message.EventDate.Offset.Hours.ToString(),
                message.TelemetryType.ToString(),
                message.EventSourceName,
                message.EventName,
                message.Message,
                message.Duration == null ? null : $"{{ Duration={TimeSpan.FromMilliseconds((long)message.Duration)} }}",
                message.Value == null ? null : $"{{ Value={message.Value} }}",
                "{ " + (message.WorkContext.ActivityId.ToString() + (message.TelemetryType.IsReplay() ? "(R)" : string.Empty)) + " }",
                "{ " + (message.WorkContext.ParentActivityId.ToString() + (message.TelemetryType.IsReplay() ? "(R)" : string.Empty)) + " }",
                message.EventDimensions == null ? null : "{ " + string.Join(", ", message.EventDimensions.Select(x => x.Key + "=" + x.Value?.ToString())) + " }",
            };

            ConsoleColor? setColor = null;
            do
            {
                if (message.TelemetryType.IsReplay())
                {
                    setColor = ConsoleColor.Yellow;
                    break;
                }

                if (message.TelemetryType.IsMetric())
                {
                    setColor = ConsoleColor.Green;
                    break;
                }

                switch (message.TelemetryType)
                {
                    case TelemetryType.Error:
                    case TelemetryType.Critical:
                        setColor = ConsoleColor.Red;
                        break;

                    case TelemetryType.Warning:
                        setColor = ConsoleColor.Yellow;
                        break;

                    case TelemetryType.Event:
                    case TelemetryType.CriticalEvent:
                    case TelemetryType.InformationEvent:
                        setColor = ConsoleColor.Cyan;
                        break;
                }
            } while (false);

            lock (_lock)
            {
                if (setColor != null) Console.ForegroundColor = (ConsoleColor)setColor;

                Console.WriteLine(string.Join(", ", list.Where(x => !x.IsEmpty())));

                if (message.Exception != null)
                {
                    setColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = (ConsoleColor)setColor;
                    Console.WriteLine(message.Exception.ToString());
                }

                if (setColor != null) Console.ResetColor();
            }
        }
    }
}
