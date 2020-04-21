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

        public override bool Equals(object? obj)
        {
            return obj is MessageClaim claim &&
                   Role.Equals(claim.Role, StringComparison.OrdinalIgnoreCase) &&
                   Value.Equals(claim.Value, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsRole(string role)
        {
            role.VerifyNotEmpty(nameof(role));

            return Role.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => HashCode.Combine(Role, Value);

        public static bool operator ==(MessageClaim v1, MessageClaim v2) => v1?.Equals(v2) == true;

        public static bool operator !=(MessageClaim v1, MessageClaim v2) => !(v1 == v2);
    }
}
