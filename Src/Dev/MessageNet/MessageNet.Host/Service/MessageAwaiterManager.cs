// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    /// <summary>
    /// Manager to handle TCS for messages that expect responses
    /// </summary>
    public class MessageAwaiterManager : IMessageAwaiterManager
    {
        private readonly ConcurrentDictionary<Guid, Registration> _completion = new ConcurrentDictionary<Guid, Registration>();
        private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(100);

        public MessageAwaiterManager() { }

        public MessageAwaiterManager(TimeSpan defaultTimeout)
        {
            _defaultTimeout = defaultTimeout;
        }

        public void Add(Guid id, TaskCompletionSource<NetMessage> tcs, TimeSpan? timeout = null)
        {
            tcs.Verify(nameof(tcs)).IsNotNull();

            timeout ??= _defaultTimeout;
            var cancellationTokenSource = new CancellationTokenSource((TimeSpan)timeout);
            cancellationTokenSource.Token.Register(() => SetException(id, new TimeoutException($"MessageNet: response was not received within timeout: {timeout.ToString()}")));

            _completion[id] = new Registration(tcs, cancellationTokenSource);
        }

        /// <summary>
        /// Set the result on the TCS waiting for a response, id must be in the 2nd header, the response
        /// </summary>
        /// <param name="netMessage"></param>
        /// <returns>true for processed, false if not</returns>
        public bool SetResult(NetMessage netMessage)
        {
            netMessage.Verify(nameof(netMessage)).IsNotNull();

            if (netMessage.Headers.Count < 2) return false;
            var header = netMessage.Headers[1];

            if (_completion.TryRemove(header.MessageId, out Registration? registration))
            {
                try { registration.Tcs.SetResult(netMessage); }
                finally { registration.Dispose(); }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set exception on the TCS waiting for a response
        /// </summary>
        /// <param name="netMessage">original net message</param>
        /// <param name="exception">exception</param>
        public void SetException(Guid id, Exception exception)
        {
            exception.Verify(nameof(exception)).IsNotNull();

            Registration registration;

            if (_completion.TryRemove(id, out registration!))
            {
                try { registration.Tcs.SetException(exception); }
                finally { registration.Dispose(); }
            }
        }

        private class Registration : IDisposable
        {
            public Registration(TaskCompletionSource<NetMessage> tcs, CancellationTokenSource tokenSource)
            {
                Tcs = tcs;
                TokenSource = tokenSource;
            }

            public TaskCompletionSource<NetMessage> Tcs { get; }

            public CancellationTokenSource TokenSource { get; }

            public void Dispose()
            {
                ((IDisposable)TokenSource).Dispose();
            }
        }
    }
}
