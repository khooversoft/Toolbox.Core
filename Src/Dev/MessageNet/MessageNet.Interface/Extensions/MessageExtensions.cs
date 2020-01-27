using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class MessageExtensions
    {

        public static MessageContent CreateMessageContent<T>(this T subject) where T : class
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new MessageContent<T>(subject);
        }

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

        public static string ToJson(this NetMessage netMessage)
        {
            netMessage.Verify(nameof(netMessage)).IsNotNull();

            return JsonConvert.SerializeObject(netMessage);
        }

        public static NetMessage ToNetMessage(this string json)
        {
            json.Verify(nameof(json)).IsNotEmpty();

            return JsonConvert.DeserializeObject<NetMessage>(json);
        }
    }
}
