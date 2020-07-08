// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Token value extracted from data
    /// </summary>
    public struct TokenValue : IToken
    {
        public TokenValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override bool Equals(object? obj) => obj is TokenValue value && Value == value.Value;

        public override int GetHashCode() => HashCode.Combine(Value);

        public override string ToString() => Value;

        public static bool operator ==(TokenValue left, TokenValue right) => left.Equals(right);

        public static bool operator !=(TokenValue left, TokenValue right) => !(left == right);
    }
}
