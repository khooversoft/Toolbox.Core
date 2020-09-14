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
            subject.VerifyNotNull(nameof(subject));

            return subject.ContentType switch
            {
                string stringType when stringType == typeof(string).Name && typeof(T) == typeof(string) => subject.Content.CastAs<T>(),

                _ => JsonConvert.DeserializeObject<T>(subject.Content),
            };
        }
    }
}
