using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public struct TokenValue : IToken
    {
        public TokenValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString() => Value;

        public override bool Equals(object obj) => (obj is TokenValue value) && value.Value == Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(TokenValue left, TokenValue right) => left.Equals(right);

        public static bool operator !=(TokenValue left, TokenValue right) => !(left == right);
    }
}
