// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class NetMessageConvertExtensions
    {
        public static readonly string _netMessageType = "net.message";

        public static Message ConvertTo(this NetMessage subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new Message(Encoding.UTF8.GetBytes(subject.ToJson()))
            {
                To = subject.Header.ToUri,
                ReplyTo = subject.Header.FromUri,
                ContentType = _netMessageType,
                CorrelationId = subject.Activity?.ActivityId.ToString(),
                MessageId = subject.Header.MessageId.ToString(),
            };
        }

        public static bool IsNetMessage(this Message subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return subject.ContentType == _netMessageType;
        }

        public static NetMessage ConvertTo(this Message subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();
            subject.IsNetMessage().Verify().Assert(x => x == true, $"MesageType is not {_netMessageType}");

            return Encoding.UTF8.GetString(subject.Body).ToNetMessage();
        }
    }
}
