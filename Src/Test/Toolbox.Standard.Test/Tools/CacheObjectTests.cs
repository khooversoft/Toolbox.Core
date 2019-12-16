// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    [Trait("Category", "Toolbox")]
    public class CacheObjectTests
    {
        [Fact]
        public void GivenCacheObject_Initialize_ShouldBeFalseState()
        {
            var cache = new CacheObject<string>(TimeSpan.FromSeconds(10));

            cache.IsValid().Should().BeFalse();
            cache.TryGetValue(out string value).Should().BeFalse();
            value.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GivenNullValue_WhenCached_ShouldBeTure()
        {
            string item = null!;
            var cache = item.ToCacheObject(TimeSpan.FromSeconds(10));

            cache.IsValid().Should().BeTrue();
            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().BeNullOrEmpty();

            string item2 = "this is the item";
            cache.Set(item2);
            cache.TryGetValue(out string value2).Should().BeTrue();
            value2.Should().Be(item2);
            cache.IsValid().Should().BeTrue();
        }

        [Fact]
        public void GivenValue_WhenCachedAndCleard_ShouldBeFalseState()
        {
            const string valueToCache = "value to be cached";
            string item = valueToCache;
            var cache = item.ToCacheObject(TimeSpan.FromSeconds(10));

            cache.IsValid().Should().BeTrue();
            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().Be(valueToCache);

            cache.Clear();
            cache.TryGetValue(out string value2).Should().BeFalse();
            value2.Should().BeNull();
        }

        [Fact]
        public void EmptyExtensionTest()
        {
            string item = null!;
            var cache = item.ToCacheObject(TimeSpan.FromSeconds(10));

            cache.IsValid().Should().BeTrue();
            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().BeNullOrEmpty();

            string item2 = "this is the item";
            cache.Set(item2);
            cache.TryGetValue(out string value2).Should().BeTrue();
            value2.Should().Be(item2);
            cache.IsValid().Should().BeTrue();
        }

        [Fact]
        public void ResetTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromMilliseconds(100)).Set(item);

            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);
            cache.IsValid().Should().BeTrue();

            cache.Clear();
            cache.IsValid().Should().BeFalse();
        }

        [Fact]
        public void StoreTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromSeconds(100)).Set(item);

            cache.IsValid().Should().BeTrue();
            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);
        }

        [Fact]
        public void StoreExtensionTest()
        {
            string item = "Item to be cached";
            CacheObject<string> cache = item.ToCacheObject(TimeSpan.FromSeconds(100));

            cache.IsValid().Should().BeTrue();
            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);
        }

        [Fact]
        public void ExpireTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromMilliseconds(100)).Set(item);

            cache.TryGetValue(out string value).Should().BeTrue();
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);

            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            cache.TryGetValue(out value).Should().BeFalse();
            value.Should().BeNullOrEmpty();
        }

        [Fact]
        public void TryTest()
        {
            string item = "Item to be cached";
            var cache = new CacheObject<string>(TimeSpan.FromMilliseconds(100)).Set(item);

            cache.TryGetValue(out string value).Should().Be(true);
            value.Should().NotBeNullOrEmpty();
            value.Should().Be(item);

            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            cache.TryGetValue(out value).Should().BeFalse();
            value.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GivenInt_WhenSetToDefault_ShouldFails()
        {
            int item = default;
            var cache = new CacheObject<int>(TimeSpan.FromMilliseconds(100)).Set(item);

            cache.TryGetValue(out int value).Should().Be(true);
            value.Should().Be(default);
        }

        [Fact]
        public void GivenInt_WhenSetToValue_ShouldPass()
        {
            int item = 10;
            var cache = new CacheObject<int>(TimeSpan.FromMilliseconds(100)).Set(item);

            cache.TryGetValue(out int value).Should().Be(true);
            value.Should().Be(item);

            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            cache.TryGetValue(out value).Should().BeFalse();
            value.Should().Be(default);
        }
    }
}
