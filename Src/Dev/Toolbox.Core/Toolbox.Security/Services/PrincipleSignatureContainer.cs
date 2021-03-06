﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Security
{
    public class PrincipleSignatureContainer : IPrincipleSignatureContainer, IEnumerable<PrincipleSignature>
    {
        private readonly ConcurrentDictionary<string, PrincipleSignature> _registration = new ConcurrentDictionary<string, PrincipleSignature>(StringComparer.OrdinalIgnoreCase);

        public PrincipleSignatureContainer()
        {
        }

        public PrincipleSignature this[string issuer] => _registration[issuer];

        public void Clear()
        {
            _registration.Clear();
        }

        public IPrincipleSignatureContainer Add(params PrincipleSignature[] principleSignature)
        {
            principleSignature
                .ForEach(x => _registration[x.Issuer] = x);

            return this;
        }

        public PrincipleSignature? Get(string issuer)
        {
            if (!_registration.TryGetValue(issuer, out PrincipleSignature principleSignature))
            {
                return null;
            }

            return principleSignature;
        }

        public IEnumerator<PrincipleSignature> GetEnumerator()
        {
            return _registration.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _registration.Values.GetEnumerator();
        }
    }
}
