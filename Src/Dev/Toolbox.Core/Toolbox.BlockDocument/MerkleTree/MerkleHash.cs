using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public struct MerkleHash
    {
        public MerkleHash(byte[] buffer)
        {
            buffer.Verify(nameof(buffer)).IsNotNull();

            SHA256 sha256 = SHA256.Create();
            Value = sha256.ComputeHash(buffer);
        }

        public MerkleHash(string buffer)
            : this(Encoding.UTF8.GetBytes(buffer))
        {
        }

        public MerkleHash(MerkleHash left, MerkleHash right)
            : this(left.Value.Concat(right.Value).ToArray())
        {
        }

        public byte[] Value { get; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            obj.Verify(nameof(obj))
                .IsNotNull()
                .Assert(x => x is MerkleHash, "rvalue is not a MerkleHash");

            return Equals((MerkleHash)obj);
        }

        public override string ToString()
        {
            return BitConverter.ToString(Value).Replace("-", "");
        }

        public bool Equals(byte[] hash)
        {
            return Value.SequenceEqual(hash);
        }

        public bool Equals(MerkleHash hash)
        {
            return Value.SequenceEqual(hash.Value);
        }

        public static bool operator ==(MerkleHash h1, MerkleHash h2)
        {
            return h1.Equals(h2);
        }

        public static bool operator !=(MerkleHash h1, MerkleHash h2)
        {
            return !h1.Equals(h2);
        }
    }
}
