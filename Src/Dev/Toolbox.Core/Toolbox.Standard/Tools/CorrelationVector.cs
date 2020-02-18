﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Immutable correlation tag value.  Can be extended or incremented.
    /// 
    /// Correlation vectors are used to trace transactions (API, REST, etc...).  Transactions are a flow of activities that are associated with each other.
    /// 
    /// Correlation vector starts with a guid (based 64) and an extent
    /// CV are extended with a transactions moves from one service to another (e.g. REST API call to another service)
    /// CV are incremented when a transaction moves to another internal process such as splitting work to multiple tasks (threads), each being processed in parallel.
    /// 
    /// </summary>
    [DebuggerDisplay("Value={Value}, BaseCv={BaseCv}")]
    public sealed class CorrelationVector
    {
        private static readonly char[] _splitSearch = new char[] { '.' };
        private readonly Deferred<string> _getFullTag;

        /// <summary>
        /// Create default CV
        /// </summary>
        public CorrelationVector()
        {
            BaseCv = GetUniqueValue();
            _getFullTag = new Deferred<string>(() => BaseCv + "." + ExtensionValue);
        }

        /// <summary>
        /// Create CV from GUID
        /// </summary>
        /// <param name="key">GUID</param>
        public CorrelationVector(Guid key)
            : this()
        {
            BaseCv = GetUniqueValue(key);
        }

        /// <summary>
        /// Parse and create CV (usually received from a calling transaction)
        /// </summary>
        /// <param name="correlationTag">CV to parse</param>
        public CorrelationVector(string correlationTag)
            : this()
        {
            correlationTag.Verify(nameof(correlationTag)).IsNotEmpty();

            string[] parts = correlationTag.Split(_splitSearch, StringSplitOptions.RemoveEmptyEntries);

            int extensionValue = 0;
            Verify.Assert<FormatException>(
                parts.Length < 2 ||
                parts.Skip(1).All(x => int.TryParse(x, out extensionValue)),
                "Data in correlation tag is not formatted correctly");

            ExtensionValue = extensionValue;
            BaseCv = string.Join(".", parts.Take(parts.Length - 1));
        }

        private CorrelationVector(string correlationTag, int extensionValue)
            : this()
        {
            correlationTag.Verify(nameof(correlationTag)).IsNotEmpty();

            BaseCv = correlationTag;
            ExtensionValue = extensionValue;
        }

        public int ExtensionValue { get; }

        public string BaseCv { get; }

        public string Value { get { return _getFullTag.Value; } }

        public override string ToString()
        {
            return Value;
        }

        public bool IsExtent(string value)
        {
            return IsExtent(ToString(), value);
        }

        /// <summary>
        /// Test if value is an extent or match if source, used to understand lineage
        /// </summary>
        /// <param name="source">source or base cv</param>
        /// <param name="value">value to test</param>
        /// <returns>true if value is an extent or match of source</returns>
        public static bool IsExtent(string source, string value)
        {
            return source.Length <= value.Length &&
                source == value.Substring(0, source.Length);
        }

        /// <summary>
        /// Implicit conversion from CV to string
        /// </summary>
        /// <param name="correlationTag"></param>
        public static implicit operator string(CorrelationVector correlationTag)
        {
            return correlationTag.Value;
        }

        /// <summary>
        /// Extend CV when being passed to another system
        /// </summary>
        /// <returns>new CV</returns>
        public CorrelationVector WithExtend()
        {
            return new CorrelationVector(Value, 0);
        }

        /// <summary>
        /// Extend CV for parallel processing internally
        /// </summary>
        /// <returns>new CV</returns>
        public CorrelationVector WithIncrement()
        {
            return new CorrelationVector(BaseCv, ExtensionValue + 1);
        }

        /// <summary>
        /// This method gets a guid and turns them into a base64 string, 22 char without padding for V2
        /// </summary>
        /// <returns> char Base 64 string</returns>
        private string GetUniqueValue(Guid? guid = null)
        {
            var allBytes = (guid ?? Guid.NewGuid()).ToByteArray();

            return Convert.ToBase64String(allBytes).Substring(0, 22)
                .Replace("/", "_");
        }
    }
}
