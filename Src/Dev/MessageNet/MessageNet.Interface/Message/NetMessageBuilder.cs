using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    /// <summary>
    /// Build net message from message item such as header, activity, content.
    /// </summary>
    public class NetMessageBuilder : IEnumerable<INetMessageItem>
    {
        private readonly List<INetMessageItem> _messageItems = new List<INetMessageItem>();

        public NetMessageBuilder() { }

        /// <summary>
        /// Add message part
        /// </summary>
        /// <param name="messageItems">message items</param>
        /// <returns>this</returns>
        public NetMessageBuilder Add(params INetMessageItem[]? messageItems)
        {
            if (messageItems == null) return this;

            _messageItems.AddRange(messageItems.Where(x => x != null));
            return this;
        }

        public IEnumerator<INetMessageItem> GetEnumerator() => _messageItems.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _messageItems.GetEnumerator();

        public NetMessage Build() => new NetMessage(_messageItems);

        /// <summary>
        /// Create net message
        /// </summary>
        /// <param name="toUri">to URI</param>
        /// <param name="fromUri">from URI</param>
        /// <param name="method">method</param>
        /// <param name="content">content (optional)</param>
        /// <returns>new net message</returns>
        public static NetMessage Create(string toUri, string fromUri, string method, string? content = null)
        {
            return new NetMessageBuilder()
                .Add(new MessageHeader(toUri.ToMessageUri(), fromUri.ToMessageUri(), method))
                .Add(content switch { string value => new MessageContent<string>(value), _ => null! })
                .Build();
        }

        public static NetMessage Create<T>(MessageUri toUri, MessageUri fromUri, string method, T? content = null) where T : class
        {
            return new NetMessageBuilder()
                .Add(new MessageHeader(toUri, fromUri, method))
                .Add(content switch { T value => new MessageContent<T>(value), _ => null! })
                .Build();
        }
    }
}
