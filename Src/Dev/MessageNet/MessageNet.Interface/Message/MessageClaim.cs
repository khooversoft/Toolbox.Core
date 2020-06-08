using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class MessageClaim
    {
        public MessageClaim(string role, string value)
        {
            Role = role;
            Value = value;
        }

        public string Role { get; }

        public string Value { get; }

        public bool IsRole(string role)
        {
            role.VerifyNotEmpty(nameof(role));

            return Role.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => HashCode.Combine(Role, Value);

        public override bool Equals(object? obj) => obj is MessageClaim claim &&
                   Role.Equals(claim.Role, StringComparison.OrdinalIgnoreCase) &&
                   Value.Equals(claim.Value, StringComparison.OrdinalIgnoreCase);

        public static bool operator ==(MessageClaim? left, MessageClaim? right) => EqualityComparer<MessageClaim>.Default.Equals(left!, right!);

        public static bool operator !=(MessageClaim? left, MessageClaim? right) => !(left == right);
    }
}
