// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class NetMessageConvertExtensions
    {
        public static readonly string _netMessageType = "net.message";

        public static Message ToMessage(this NetMessage subject)
        {
            subject.VerifyNotNull(nameof(subject));

            NetMessageModel netMessageModel = subject.ConvertTo();
            string json = JsonConvert.SerializeObject(netMessageModel);
            byte[] data = Encoding.UTF8.GetBytes(json);

            return new Message(data)
            {
                To = subject.Header.ToUri,
                ReplyTo = subject.Header.FromUri,
                ContentType = _netMessageType,
                CorrelationId = subject.Activity?.ActivityId.ToString(),
                MessageId = subject.Header.MessageId.ToString(),
            };
        }

        public static NetMessage ToNetMessage(this Message subject)
        {
            subject.VerifyNotNull(nameof(subject));

            string json = Encoding.UTF8.GetString(subject.Body);
            NetMessageModel model = JsonConvert.DeserializeObject<NetMessageModel>(json);
            return model.ConvertTo();
        }
    }
}
