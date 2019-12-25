// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System.Linq;

namespace Khooversoft.Toolbox.Standard
{
    public static class TelemetryMessageExtensions
    {
        public static string ConvertToJson(this TelemetryMessage message)
        {
            var model = new TelemetryMessageModel
            {
                MessageId = message.MessageId,
                EventDate = message.EventDate,
                Cv = message.WorkContext.Cv,
                Tag = message.WorkContext.Tag,
                TelemetryType = message.TelemetryType,
                EventSourceName = message.EventSourceName,
                EventName = message.EventName,
                Message = message.Message,
                Duration = message.Duration,
                Value = message.Value,
                Dimensions = message.EventDimensions.ToDictionary(x => x.Key, x => x.Value),
                Exception = message.Exception,
            };

            return JsonConvert.SerializeObject(model);

            //return JsonSerializer.Serialize(model);
        }

        public static TelemetryMessage? ConvertJsonToTelemetryMessage(this string jsonMessage)
        {
            if (jsonMessage.IsEmpty())
            {
                return null;
            }

            //var md = JsonSerializer.Deserialize<TelemetryMessageModel>(jsonMessage);
            var md = JsonConvert.DeserializeObject<TelemetryMessageModel>(jsonMessage);
            md.Verify(nameof(md)).IsNotNull();

            IWorkContext context = new WorkContextBuilder()
            {
                Cv = md.Cv != null ? new CorrelationVector(md.Cv) : new CorrelationVector(),
                Tag = md.Tag != null ? StringVector.Parse(md.Tag) : StringVector.Empty,
            }.Build();

            return new TelemetryMessage(md, context);
        }
    }
}
