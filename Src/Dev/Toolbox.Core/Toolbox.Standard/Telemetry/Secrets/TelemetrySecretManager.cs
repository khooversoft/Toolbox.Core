// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Utility to mask secrets
    /// </summary>
    public class TelemetrySecretManager : ITelemetrySecretManager
    {
        private readonly HashSet<string> _secrets = new HashSet<string>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public TelemetrySecretManager()
        {
        }

        public int Count { get => _secrets.Count; }

        public TelemetrySecretManager Add(params string[] secrets)
        {
            secrets.Verify(nameof(secrets)).IsNotNull();

            _lock.EnterWriteLock();
            try
            {
                secrets
                    .ForEach(x => _secrets.Add(x));
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return this;
        }

        public string? Mask(string? value)
        {
            _lock.EnterReadLock();

            try
            {
                if (_secrets.Count == 0 || value.IsEmpty() || !_secrets.Any(x => value!.IndexOf(x) >= 0))
                {
                    return value;
                }

                return _secrets
                    .Aggregate(value!, (a, v) => a.Replace(v, "***"));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public ITelemetrySecretManager With(ITelemetrySecretManager add)
        {
            if (add == null)
            {
                return this;
            }

            TelemetrySecretManager addManager = (add as TelemetrySecretManager) ?? throw new ArgumentException($"{nameof(add)} is not 'TelemetrySecretManager'");

            return new TelemetrySecretManager().Add(_secrets.Concat(addManager._secrets).ToArray());
        }

        public IEnumerator<string> GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return _secrets
                    .ToList()
                    .GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
