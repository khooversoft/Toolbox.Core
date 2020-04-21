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
                ActivityId = message.WorkContext.ActivityId,
                ParentActivityId = message.WorkContext.ParentActivityId,
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

            var md = JsonConvert.DeserializeObject<TelemetryMessageModel>(jsonMessage);
            md.VerifyNotNull(nameof(md));

            IWorkContext context = new WorkContextBuilder()
            {
                ActivityId = md.ActivityId,
                ParentActivityId = md.ParentActivityId,
            }.Build();

            return new TelemetryMessage(md, context);
        }
    }
}
