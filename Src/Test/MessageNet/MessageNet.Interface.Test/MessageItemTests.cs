// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.MessageNet.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MessageNet.Interface.Test
{
    public class MessageItemTests
    {
        [Fact]
        public void GivenHeader_WhenCloned_ShouldBeEqual()
        {
            var expected = new MessageHeader("ns/net1/node1", "ns/net1/node2", "method");
            var subject = new MessageHeader(expected);

            subject.Should().Be(expected);
            (expected == subject).Should().BeTrue();

            (subject == null!).Should().BeFalse();
        }
        
        [Fact]
        public void GivenHeaderWithProperties_WhenCloned_ShouldBeEqual()
        {
            var expected = new MessageHeader("ns/net1/node1", "ns/net1/node2", "method", new MessageClaim("key", "value"));
            var subject = new MessageHeader(expected);

            subject.Should().Be(expected);
            (expected == subject).Should().BeTrue();

            (subject == null!).Should().BeFalse();
        }

        [Fact]
        public void GivenHeader_WhenDiffernt_ShouldNotBeEqual()
        {
            Guid messageId = Guid.NewGuid();

            var expected = new MessageHeader(messageId, "ns/net1/node1", "ns/net1/node2", "method");
            var subject = new MessageHeader(messageId, "ns/net1/node3", "ns/net1/node2", "method");

            subject.Should().NotBe(expected);
            (expected != subject).Should().BeTrue();

            (subject != null!).Should().BeTrue();
        }

        [Fact]
        public void GivenHeaderWithProperties_WhenSame_ShouldBeEqual()
        {
            Guid messageId = Guid.NewGuid();

            var expected = new MessageHeader(messageId, "ns/net1/node1", "ns/net1/node2", "method", new MessageClaim("key1", "value1"), new MessageClaim("key2", "value2"));
            var subject = new MessageHeader(messageId, "ns/net1/node1", "ns/net1/node2", "method", new MessageClaim("key1", "value1"), new MessageClaim("key2", "value2"));

            subject.Should().Be(expected);
            (expected == subject).Should().BeTrue();
        }

        [Fact]
        public void GivenHeaderWithProperties_WhenDiffernt_ShouldNotBeEqual()
        {
            Guid messageId = Guid.NewGuid();

            var expected = new MessageHeader(messageId, "ns/net1/node1", "ns/net1/node2", "method", new MessageClaim("key1", "value1"), new MessageClaim("key2", "value2"));
            var subject = new MessageHeader(messageId, "ns/net1/node3", "ns/net1/node2", "method", new MessageClaim("key1", "value1"), new MessageClaim("key2", "value2"));

            subject.Should().NotBe(expected);
            (expected != subject).Should().BeTrue();

            (subject != null!).Should().BeTrue();
        }

        [Fact]
        public void GivenHeaderWithProperties_WhenDifferntProperty_ShouldNotBeEqual()
        {
            Guid messageId = Guid.NewGuid();

            var expected = new MessageHeader(messageId, "ns/net1/node1", "ns/net1/node2", "method", new MessageClaim("key1", "value1"), new MessageClaim("key2", "value2"));
            var subject = new MessageHeader(messageId, "ns/net1/node1", "ns/net1/node2", "method", new MessageClaim("key1", "value1"), new MessageClaim("key3", "value2"));

            subject.Should().NotBe(expected);
            (expected != subject).Should().BeTrue();

            (subject != null!).Should().BeTrue();
        }

        [Fact]
        public void GivenHeader_WhenCreatedIdenticalCloned_ShouldBeEqual()
        {
            Guid messageId = Guid.NewGuid();
            var expected = new MessageHeader(messageId, "ns/net1/node1", "ns/net1/node2", "method");
            var subject = new MessageHeader(messageId, "ns/net1/node1", "ns/net1/node2", "method");

            subject.Should().Be(expected);
            (expected == subject).Should().BeTrue();

            (subject == null!).Should().BeFalse();
        }

        [Fact]
        public void GivenActivity_WhenCloned_ShouldBeEqual()
        {
            Guid activityId = Guid.NewGuid();
            Guid parentId = Guid.NewGuid();

            var expected = new MessageActivity(activityId, parentId);
            var subject = new MessageActivity(activityId, parentId);

            subject.Should().Be(expected);
            (expected == subject).Should().BeTrue();

            (subject == null!).Should().BeFalse();
        }

        [Fact]
        public void GivenActivity_WhenDifferent_ShouldNotBeEqual()
        {
            Guid activityId = Guid.NewGuid();
            Guid parentId = Guid.NewGuid();

            var expected = new MessageActivity(activityId, parentId);
            var subject = new MessageActivity(Guid.NewGuid(), parentId);

            subject.Should().NotBe(expected);
            (expected == subject).Should().BeFalse();

            (subject != null!).Should().BeTrue();
        }

        [Fact]
        public void GivenActivity_WhenWithNoParentId_ShouldBeEqual()
        {
            Guid activityId = Guid.NewGuid();

            var expected = new MessageActivity(activityId);
            var subject = new MessageActivity(activityId);

            subject.Should().Be(expected);
            (expected == subject).Should().BeTrue();

            (subject == null!).Should().BeFalse();
        }

        [Fact]
        public void GivenContent_WhenSame_ShouldBeEqual()
        {
            var expected = new MessageContent("type", "message Content #1");
            var subject = new MessageContent("type", "message Content #1");

            subject.Should().Be(expected);
            (expected == subject).Should().BeTrue();

            (subject == null!).Should().BeFalse();
        }

        [Fact]
        public void GivenContent_WhenDiffernt_ShouldNotBeEqual()
        {
            var expected = new MessageContent("type", "message Content #1");
            var subject = new MessageContent("type", "message D Content #1");

            subject.Should().NotBe(expected);
            (expected == subject).Should().BeFalse();

            (subject != null!).Should().BeTrue();
        }
    }
}
