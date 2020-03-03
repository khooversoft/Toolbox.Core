using Khooversoft.MessageNet.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MessageNet.Interface.Test
{
    public class NetMessageSerializationTests
    {
        [Fact]
        public void GivenFullMessage_WhenSerialized_ShouldDeserialize()
        {
            var header = new MessageHeader("network1/node3", "network1/node4", "post", new KeyValuePair<string, string>("key1", "value1"));
            var activity = new MessageActivity(Guid.NewGuid());
            var content = new MessageContent<string>("This is the value");
            var contentBinary = new MessageContent<byte[]>(Encoding.UTF8.GetBytes("and matches other content"));
            var classContent = new MessageContent<ContentData>(new ContentData(99, "value99"));

            NetMessage message = new NetMessageBuilder()
                .Add(header)
                .Add(activity)
                .Add(content)
                .Add(contentBinary)
                .Build();

            string json = JsonConvert.SerializeObject(message);
        }

        private class ContentData
        {
            public ContentData(int index, string value)
            {
                Index = index;
                Value = value;
            }

            public int Index { get; }
            public string Value { get; }
        }
    }
}
