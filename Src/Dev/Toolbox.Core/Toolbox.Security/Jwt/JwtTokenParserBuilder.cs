// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Toolbox.Security
{
    /// <summary>
    /// Build a JWT token parser, specify certificates, audiences, and issuers.  This is just a helper builder pattern class
    /// </summary>
    public class JwtTokenParserBuilder
    {
        public JwtTokenParserBuilder()
        {
        }

        public IDictionary<string, X509Certificate2> Certificates { get; } = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

        public IList<string> ValidIssuers { get; set; } = new List<string>();

        public IList<string> ValidAudiences { get; } = new List<string>();

        public JwtTokenParserBuilder Clear()
        {
            ValidIssuers.Clear();
            Certificates.Clear();
            ValidAudiences.Clear();

            return this;
        }

        public JwtTokenParserBuilder AddValidIssuer(params string[] validIssuer)
        {
            validIssuer.ForEach(x => ValidIssuers.Add(x));
            return this;
        }

        public JwtTokenParserBuilder AddCertificate(params X509Certificate2[] certificate)
        {
            certificate.Verify(nameof(certificate)).IsNotNull();

            certificate.ForEach(x => Certificates.Add(x.Thumbprint, x));
            return this;
        }

        public JwtTokenParserBuilder AddValidAudience(params string[] validAudience)
        {
            validAudience.Verify(nameof(validAudience)).IsNotNull();

            validAudience.ForEach(x => ValidAudiences.Add(x));
            return this;
        }

        public JwtTokenParser Build()
        {
            return new JwtTokenParser(Certificates, ValidIssuers, ValidAudiences);
        }
    }
}
