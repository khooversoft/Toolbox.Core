// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.MessageNet.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MessageNet.Interface.Test
{
    public class NetMessageTests
    {
        [Fact]
        public void GivenSimpleMessage_WhenCreatedEqual_ShouldEqual()
        {
            Guid messageId = Guid.NewGuid();
            var header1 = new MessageHeader(messageId, "network1/node2", "network1/node1", "post");
            var header2 = new MessageHeader(messageId, "network1/node2", "network1/node1", "post");

            NetMessage message1 = new NetMessageBuilder()
                .Add(header1)
                .Build();

            NetMessage message2 = new NetMessageBuilder()
                .Add(header2)
                .Build();

            message1.Should().Be(message2);
            (message1 == message2).Should().BeTrue();
        }

        [Fact]
        public void GivenSimpleMessage_WhenNotCreatedEqual_ShouldNotEqual()
        {
            var header1 = new MessageHeader("network1/node2", "network1/node1", "post");
            var header2 = new MessageHeader("network1/node9", "network1/node1", "post");

            NetMessage message1 = new NetMessageBuilder()
                .Add(header1)
                .Build();

            NetMessage message2 = new NetMessageBuilder()
                .Add(header2)
                .Build();

            message1.Should().NotBe(message2);
            (message1 == message2).Should().BeFalse();
        }

        [Fact]
        public void GivenHeaderItem_WhenBuild_ItemsShouldVerify()
        {
            var header = new MessageHeader("network1/node2", "network1/node1", "post");

            NetMessage message = new NetMessageBuilder()
                .Add(header)
                .Build();

            message.Should().NotBeNull();
            message.MessageItems.Should().NotBeNull();
            message.MessageItems.Count.Should().Be(1);

            message.MessageItems.OfType<MessageHeader>().First().ToUri.Should().Be(header.ToUri);
            message.MessageItems.OfType<MessageHeader>().First().FromUri.Should().Be(header.FromUri);
            message.MessageItems.OfType<MessageHeader>().First().Method.Should().Be(header.Method);

            message.Header.Should().NotBeNull();
            message.Header.ToUri.Should().Be(header.ToUri);
            message.Header.FromUri.Should().Be(header.FromUri);
            message.Header.Method.Should().Be(header.Method);

            message.Header.Should().Be(header);
        }
        
        [Fact]
        public void GivenMultipleHeaderItem_WhenBuild_ItemsShouldVerify()
        {
            var headers = new MessageHeader[]
            {
                new MessageHeader("network1/node1", "network1/node2", "post"),
                new MessageHeader("network1/node2", "network1/node1", "post"),
            };

            NetMessage message = new NetMessageBuilder()
                .Add(headers[0])
                .Build()
                .WithAddToTop(headers[1]);

            message.Should().NotBeNull();
            message.MessageItems.Should().NotBeNull();
            message.MessageItems.Count.Should().Be(2);

            message.MessageItems.OfType<MessageHeader>()
                .Zip(headers.Reverse(), (subject, expected) => (subject, expected))
                .All(x => x.subject == x.expected)
                .Should().BeTrue();

            message.Header.Should().NotBeNull();
            message.Header.ToUri.Should().Be(headers[1].ToUri);
            message.Header.FromUri.Should().Be(headers[1].FromUri);
            message.Header.Method.Should().Be(headers[1].Method);
        }

        [Fact]
        public void GivenHeaderAndActivityItem_WhenBuild_ItemsShouldVerify()
        {
            var header = new MessageHeader("network1/node2", "network1/node1", "post");
            var activity = new MessageActivity(Guid.NewGuid());

            NetMessage message = new NetMessageBuilder()
                .Add(header)
                .Add(activity)
                .Build();

            message.Should().NotBeNull();
            message.MessageItems.Should().NotBeNull();

            var messageItems = new INetMessageItem[]
            {
                header,
                activity,
            };

            message.MessageItems.Count.Should().Be(messageItems.Length);

            message.MessageItems
                .Zip(messageItems, (subject, expected) => (subject, expected))
                .All(x => x.subject == x.expected)
                .Should().BeTrue();

            message.Header.Should().NotBeNull();
            message.Header.ToUri.Should().Be(header.ToUri);
            message.Header.FromUri.Should().Be(header.FromUri);
            message.Header.Method.Should().Be(header.Method);

            message.Activity.Should().NotBeNull();
            message.Activity!.ActivityId.Should().Be(activity.ActivityId);
            message.Activity!.ParentActivityId.Should().Be(activity.ParentActivityId);
        }

        [Fact]
        public void GivenHeaderAndActivityAndDataItem_WhenBuild_ItemsShouldVerify()
        {
            var header = new MessageHeader("network1/node3", "network1/node4", "post");
            var activity = new MessageActivity(Guid.NewGuid());
            var content = new MessageContent("type", "This is the value");

            NetMessage message = new NetMessageBuilder()
                .Add(header)
                .Add(activity)
                .Add(content)
                .Build();

            message.Should().NotBeNull();
            message.MessageItems.Should().NotBeNull();

            var messageItems = new INetMessageItem[]
            {
                header,
                activity,
                content,
            };

            message.MessageItems.Count.Should().Be(messageItems.Length);

            message.MessageItems
                .Zip(messageItems, (subject, expected) => (subject, expected))
                .All(x => x.subject == x.expected)
                .Should().BeTrue();

            message.Header.Should().Be(header);
            message.Header.ToUri.Should().Be(header.ToUri);
            message.Header.FromUri.Should().Be(header.FromUri);
            message.Header.Method.Should().Be(header.Method);

            message.Activity.Should().NotBeNull();
            message.Activity!.ActivityId.Should().Be(activity.ActivityId);
            message.Activity!.ParentActivityId.Should().Be(activity.ParentActivityId);

            message.Content.Should().NotBeNull();
            message.Content!.ContentType.Should().Be(content.ContentType);
            message.Content!.Content.Should().Be(content.Content);
        }

        [Fact]
        public void GivenCustomMessageItem_WhenBuild_ItemsShouldVerify()
        {
            var header = new MessageHeader("network1/node3", "network1/node4", "post");
            var activity = new MessageActivity(Guid.NewGuid());
            var content = new MessageContent("type", "This is the value");

            NetMessage message = new NetMessageBuilder()
                .Add(header)
                .Add(activity)
                .Add(content)
                .Build();

            message.Should().NotBeNull();
            message.MessageItems.Should().NotBeNull();

            var messageItems = new INetMessageItem[]
            {
                header,
                activity,
                content,
            };

            message.MessageItems.Count.Should().Be(messageItems.Length);

            message.MessageItems
                .Zip(messageItems, (subject, expected) => (subject, expected))
                .All(x => x.subject == x.expected)
                .Should().BeTrue();

            message.Header.Should().NotBeNull();
            message.Header.ToUri.Should().Be(header.ToUri);
            message.Header.FromUri.Should().Be(header.FromUri);
            message.Header.Method.Should().Be(header.Method);

            message.Activity.Should().NotBeNull();
            message.Activity!.ActivityId.Should().Be(activity.ActivityId);
            message.Activity!.ParentActivityId.Should().Be(activity.ParentActivityId);

            message.Content.Should().NotBeNull();
            message.Content!.ContentType.Should().Be(content.ContentType);
            message.Content!.Content.Should().Be(content.Content);
        }
    }
}
