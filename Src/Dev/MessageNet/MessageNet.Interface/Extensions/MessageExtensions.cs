// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class MessageExtensions
    {
        public static T GetMessageContent<T>(this MessageContent subject) where T : class
        {
            subject.Verify(nameof(subject)).IsNotNull();

            switch (subject.ContentType)
            {
                case string stringType when stringType == typeof(string).Name && typeof(T) == typeof(string):
                    return subject.Content.CastAs<T>();

                default:
                    return JsonConvert.DeserializeObject<T>(subject.Content);
            }
        }
    }
}
