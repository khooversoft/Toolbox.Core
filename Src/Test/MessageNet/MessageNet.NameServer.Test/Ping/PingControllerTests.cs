// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Standard;
using System.Threading.Tasks;
using Xunit;

namespace MessageHub.NameServer.Test
{
    public class PingControllerTests : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;
        private readonly IWorkContext _workContext = WorkContextBuilder.Default;

        public PingControllerTests(TestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task WhenUsingRegistrationApi_WhenPingRequested_ReturnedOk()
        {
            var response = await _fixture.Client.GetAsync("api/Ping");

            response.EnsureSuccessStatusCode();

            var responseStrong = await response.Content.ReadAsStringAsync();

            responseStrong.Should().Be("{\"ok\":true}");
        }

        [Fact]
        public async Task WhenUsingRegistrationApiWithClient_WhenPingRequested_ReturnedOk()
        {
            string response = await new RestClient(_fixture.Client)
                .AddPath("api/Ping")
                .SetEnsureSuccessStatusCode()
                .GetAsync(_workContext)
                .GetContentAsync<string>(_workContext);

            response.Should().Be("{\"ok\":true}");
        }

        [Fact]
        public async Task WhenUsingNameServerClient_WhenPingRequested_ReturnedOk()
        {
            bool status = await new NameServerClient(_fixture.Client)
                .Ping(_workContext);

            status.Should().BeTrue();
        }
    }
}
