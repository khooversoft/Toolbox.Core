using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public struct TokenValueQuoted : IToken
    {
        public TokenValueQuoted(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString() => Value;

        public override bool Equals(object obj) => (obj is TokenValueQuoted value) && value.Value == Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(TokenValueQuoted left, TokenValueQuoted right) =>left.Equals(right);

        public static bool operator !=(TokenValueQuoted left, TokenValueQuoted right) => !(left == right);
    }
}
