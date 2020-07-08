// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.Toolbox.Security;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Toolbox.Security.Test
{
    public class JwtTests
    {
        private static readonly LocalCertificate _jwtVaultTest =
            new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "1C7E74F389E668068C9143DFF3E7DD4EF9580981", true, new NullLogger<LocalCertificate>());

        private static readonly LocalCertificate _vaultData =
            new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "388A397D47ACD9C9D75E06A4FFEE016CB7EFF0CB", true, new NullLogger<LocalCertificate>());

        [Trait("Category", "LocalOnly")]
        [Fact]
        public void JwtSecurityTokenBuilderTest()
        {
            const string userId = "user@domain.com";
            const string emailText = "Email";
            const string emailId = "testemail@domain.com";

            string token = new JwtTokenBuilder()
                .SetAudience(new Uri("http://localhost/audience").ToString())
                .SetIssuer(new Uri("http://localhost/Issuer").ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim(emailText, emailId))
                .AddSubject(userId)
                .SetCertificate(_jwtVaultTest.GetCertificate())
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate())
                .Build()
                .Parse(token);

            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(_jwtVaultTest.LocalCertificateKey.Thumbprint);
            tokenDetails.JwtSecurityToken.Subject.Should().Be(userId);
            tokenDetails.JwtSecurityToken.Claims.Any(x => x.Type == emailText && x.Value == emailId).Should().BeTrue();
        }

        [Trait("Category", "LocalOnly")]
        [Fact]
        public void JwtSecurityTokenAudienceTest()
        {
            var audience = new Uri("http://localhost/audience");

            string token = new JwtTokenBuilder()
                .SetAudience(audience.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(_jwtVaultTest.GetCertificate())
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate())
                .AddValidAudience(audience.ToString())
                .Build()
                .Parse(token);

            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(_jwtVaultTest.LocalCertificateKey.Thumbprint);
        }

        [Trait("Category", "LocalOnly")]
        [Fact]
        public void JwtSecurityTokenIssuerTest()
        {
            var issuer = new Uri("http://localhost/Issuer");

            string token = new JwtTokenBuilder()
                .SetIssuer(issuer.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(_jwtVaultTest.GetCertificate())
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate())
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(token);

            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(_jwtVaultTest.LocalCertificateKey.Thumbprint);
        }

        [Trait("Category", "LocalOnly")]
        [Fact]
        public void JwtSecurityFailureTest()
        {
            var issuer = new Uri("http://localhost/Issuer");

            string token = new JwtTokenBuilder()
                .SetIssuer(issuer.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(_jwtVaultTest.GetCertificate())
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_vaultData.GetCertificate())
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(token);

            // Failure
            tokenDetails.Should().BeNull();

            tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate())
                .AddCertificate(_vaultData.GetCertificate())
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(token);

            tokenDetails.Should().NotBeNull();
            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(_jwtVaultTest.LocalCertificateKey.Thumbprint);
        }

        [Trait("Category", "LocalOnly")]
        [Fact]
        public void JwtSecuritySignatureFailureTest()
        {
            var issuer = new Uri("http://localhost/Issuer");

            string token = new JwtTokenBuilder()
                .SetIssuer(issuer.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(_jwtVaultTest.GetCertificate())
                .Build();

            token.Should().NotBeNullOrEmpty();

            token = token.Remove(3, 2);

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate())
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(token);

            tokenDetails.Should().BeNull();
        }
    }
}
