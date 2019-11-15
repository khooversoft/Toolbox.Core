// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Khooversoft.Toolbox.Standard
{
    public static class TelemetryMessageExtensions
    {
        public static string ConvertToJson(this TelemetryMessage message)
        {
            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);

            writer.WriteStartObject();

            writer.WritePropertyName(nameof(message.TelemetryType));
            writer.WriteValue(message.TelemetryType.ToString());

            writer.WritePropertyName(nameof(message.EventDate));
            writer.WriteValue(message.EventDate.ToString("o"));

            writer.WritePropertyName(nameof(message.WorkContext.Cv));
            writer.WriteValue(message.WorkContext.Cv);

            writer.WritePropertyName(nameof(message.WorkContext.Tag));
            writer.WriteValue(message.WorkContext.Tag);

            writer.WritePropertyName(nameof(message.EventSourceName));
            writer.WriteValue(message.EventSourceName);

            writer.WritePropertyName(nameof(message.EventName));
            writer.WriteValue(message.EventName);

            if (message.Message != null)
            {
                writer.WritePropertyName(nameof(message.Message));
                writer.WriteValue(message.Message);
            }

            if (message.Duration != null)
            {
                writer.WritePropertyName(nameof(message.Duration));
                writer.WriteValue(message.Duration);
            }

            if (message.Value != null)
            {
                writer.WritePropertyName(nameof(message.Value));
                writer.WriteValue(message.Value);
            }

            if (message.Exception != null)
            {
                writer.WritePropertyName(nameof(message.Exception));
                writer.WriteValue(message.Exception.ToString());
            }

            if (message.EventDimensions != null)
            {
                writer.WritePropertyName("Dimensions");
                writer.WriteStartObject();
                message.EventDimensions.ForEach(x => { writer.WritePropertyName(x.Key); writer.WriteValue(x.Value); });
                writer.WriteEndObject();
            }

            writer.WriteEndObject();

            return sw.ToString();
        }

        public static TelemetryMessage? ConvertJsonToTelemetryMessage(this string jsonMessage)
        {
            if (jsonMessage.IsEmpty())
            {
                return null;
            }

            var md = JsonConvert.DeserializeObject<MessageDeserialize>(jsonMessage);
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
