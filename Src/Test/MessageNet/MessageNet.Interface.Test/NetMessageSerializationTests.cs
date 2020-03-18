using FluentAssertions;
using Khooversoft.MessageNet.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MessageNet.Interface.Test
{
    public class NetMessageSerializationTests
    {
        [Fact]
        public void GivenMessageHeader_WhenSerialized_ShouldMatch()
        {
            var subject = new MessageHeader("ns/netid/node", "ns/netid/node", "post", new MessageClaim("key1", "value1"));

            MessageHeaderModel model = subject.ConvertTo();
            string json = JsonConvert.SerializeObject(model);

            MessageHeaderModel result = JsonConvert.DeserializeObject<MessageHeaderModel>(json);
            MessageHeader resultHeader = result.ConvertTo();

            subject.Should().Be(resultHeader);
        }
        
        [Fact]
        public void GivenMessageActivity_WhenSerialized_ShouldMatch()
        {
            var subject = new MessageActivity(Guid.NewGuid(), Guid.NewGuid());

            MessageActivityModel model = subject.ConvertTo();
            string json = JsonConvert.SerializeObject(model);

            MessageActivityModel result = JsonConvert.DeserializeObject<MessageActivityModel>(json);
            MessageActivity resultActivity = result.ConvertTo();

            subject.Should().Be(resultActivity);
        }
        
        [Fact]
        public void GivenMessageContent_WhenSerialized_ShouldMatch()
        {
            var subject = new MessageContent("type1", "String type");

            MessageContentModel model = subject.ConvertTo();
            string json = JsonConvert.SerializeObject(model);

            MessageContentModel result = JsonConvert.DeserializeObject<MessageContentModel>(json);
            MessageContent resultActivity = result.ConvertTo();

            subject.Should().Be(resultActivity);
        }

        [Fact]
        public void GivenFullMessage_WhenSerialized_ShouldDeserialize()
        {
            var header = new MessageHeader("ns/network1/node3", "ns/network1/node4", "post", new MessageClaim("key1", "value1"));
            var activity = new MessageActivity(Guid.NewGuid());
            var content = new MessageContent("string", "This is the value");
            var contentBinary = new MessageContent("match", "and matches other content");
            var classContent = MessageContent.Create(new ContentData(99, "value99"));

            NetMessage message = new NetMessageBuilder()
                .Add(header)
                .Add(activity)
                .Add(content)
                .Add(contentBinary)
                .Add(classContent)
                .Build();

            NetMessageModel netMessageModel = message.ConvertTo();
            netMessageModel.Should().NotBeNull();

            NetMessage netMessage = netMessageModel.ConvertTo();
            netMessage.Should().NotBeNull();
            netMessage.Version.Should().Be("1.0.0.0");

            netMessage.Headers!.Count.Should().Be(1);
            netMessage.Headers!.Single().Should().Be(header);

            netMessage.Activities!.Count.Should().Be(1);
            netMessage.Activities!.Single().Should().Be(activity);

            netMessage.Contents!.Count.Should().Be(3);
            netMessage.Contents!.First().Should().Be(content);
            netMessage.Contents!.Skip(1).First().Should().Be(contentBinary);
            netMessage.Contents!.Skip(2).First().Should().Be(classContent);
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
