// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Actor key, a GUID is created from the string vector
    /// </summary>
    public class ActorKey
    {
        /// <summary>
        /// Construct actor key from vector
        /// </summary>
        /// <param name="vectorKey"></param>
        public ActorKey(string vectorKey)
        {
            vectorKey.Verify(nameof(vectorKey)).IsNotNull();

            VectorKey = vectorKey;
            Key = VectorKey.ToLowerInvariant().ToGuid();
        }

        public static ActorKey Default { get; } = new ActorKey("default");

        /// <summary>
        /// Actor key (hash from vector key)
        /// </summary>
        public Guid Key { get; }

        /// <summary>
        /// Vector key
        /// </summary>
        public string VectorKey { get; }

        public override string ToString() => $"Key={Key}, Vector key={VectorKey}";

        public override bool Equals(object obj)
        {
            ActorKey? subject = obj as ActorKey;
            if (obj == null) return false;

            return Key == subject!.Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public static bool operator ==(ActorKey v1, ActorKey v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(ActorKey v1, ActorKey v2)
        {
            return !v1.Equals(v2);
        }

        /// <summary>
        /// Implicit convert actor key to string
        /// </summary>
        /// <param name="actorKey"></param>
        public static explicit operator string(ActorKey actorKey)
        {
            actorKey.Verify(nameof(actorKey)).IsNotNull();

            return actorKey.VectorKey;
        }
    }
}
