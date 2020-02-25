// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessageHub.NameServer.Test.Registration
{
    public class RegistrationContollerTests : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;
        private readonly IWorkContext _workContext = WorkContextBuilder.Default;

        public RegistrationContollerTests(TestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task WhenNodeRegistrating_GivenTestNode_ShouldRegister()
        {
            await _fixture.NameServerClient.ClearAll(_workContext);

            var request = RouteRequest.Test("test/Node1");
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var httpResponse = await _fixture.Client
                .PostAsync("api/Registration", content);

            httpResponse.EnsureSuccessStatusCode();

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            RouteResponse response = JsonConvert.DeserializeObject<RouteResponse>(responseString);

            response.Should().Be("test/Node1");
        }

        [Fact]
        public async Task WhenNodeRegistrating_GivenRestClient_ShouldRegister()
        {
            await _fixture.NameServerClient.ClearAll(_workContext);

            var request = RouteRequest.Test("test/Node1");

            RouteResponse response = await _fixture.NameServerClient.Register(_workContext, request);

            response.Namespace.Should().NotBeEmpty();
            response.NetworkId.Should().Be("test");
            response.NodeId.Should().Be("Node1");
        }

        [Fact]
        public async Task WhenNodeNotExist_GivenLookup_ShouldFail()
        {
            await _fixture.NameServerClient.ClearAll(_workContext);

            var request = new RouteRequest { NodeId = "test/Node1" };

            RouteResponse? response = await _fixture.NameServerClient.Lookup(_workContext, request);
            response.Should().BeNull();
        }

        [Fact]
        public async Task WhenNodeRegistered_GivenUnregister_ShouldPass()
        {
            await _fixture.NameServerClient.ClearAll(_workContext);

            var request = RouteRequest.Test("test/Node1");

            RouteResponse response = await _fixture.NameServerClient.Register(_workContext, request);

            var lookupRequest = new RouteRequest { NodeId = request.NodeId };

            RouteResponse? lookupResponse = await _fixture.NameServerClient.Lookup(_workContext, lookupRequest);
            lookupResponse.Should().NotBeNull();
            lookupResponse!.NodeId.Should().Be(request.NodeId);

            await _fixture.NameServerClient.Unregister(_workContext, request);

            lookupResponse = await _fixture.NameServerClient.Lookup(_workContext, lookupRequest);
            lookupResponse.Should().BeNull();
        }
    }
}
