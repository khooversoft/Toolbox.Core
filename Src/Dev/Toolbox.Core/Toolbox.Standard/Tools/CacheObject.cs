// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Cache object, valid for only a specific amount of time specified in lifetime.
    /// If specified, refresh, can indicate when a refresh operations should be done.
    /// 
    /// This object is thread protected by using non-locking methods.
    /// </summary>
    /// <typeparam name="T">type of object cached</typeparam>
    public class CacheObject<T>
    {
        private ValueStore _valueStore;

        /// <summary>
        /// Construct a cache
        /// </summary>
        /// <param name="lifeTime">cache lifetime</param>
        /// <param name="refresh">refresh lifetime (optional)</param>
        public CacheObject(TimeSpan lifeTime)
        {
            LifeTime = lifeTime;
        }

        /// <summary>
        /// Cache lifetime setting
        /// </summary>
        public TimeSpan LifeTime { get; }

        /// <summary>
        /// Clear cache, clear value and timeouts
        /// </summary>
        /// <returns>this</returns>
        public CacheObject<T> Clear()
        {
            _valueStore = default;
            return this;
        }

        /// <summary>
        /// Try to get value
        /// </summary>
        /// <param name="value">return value</param>
        /// <returns>true if value is available, false if cache has expired</returns>
        public bool TryGetValue(out T value)
        {
            ValueStore current = _valueStore;
            value = current.Value;

            if (DateTimeOffset.Now < current.ValidTo) return true;

            value = default!;
            if (EqualityComparer<T>.Equals(current, default)) return false;

            _valueStore = default;
            return false;
        }

        /// <summary>
        /// Set value and reset clocks
        /// </summary>
        /// <param name="value">value to set</param>
        /// <returns>this</returns>
        public CacheObject<T> Set(T value)
        {
            _valueStore = new ValueStore(value, DateTime.Now + LifeTime);
            return this;
        }

        /// <summary>
        /// Is cache valid
        /// </summary>
        /// <returns>true if valid, false if expired</returns>
        public bool IsValid()
        {
            ValueStore current = _valueStore;

            return !EqualityComparer<T>.Equals(current, default) &&
                DateTimeOffset.Now < current.ValidTo;
        }

        /// <summary>
        /// Structure to store value with valid to calculated
        /// </summary>
        private struct ValueStore
        {
            public ValueStore(T value, DateTime validTo)
            {
                Value = value;
                ValidTo = validTo;
            }

            public DateTime ValidTo { get; }

            public T Value { get; }
        }
    }
}