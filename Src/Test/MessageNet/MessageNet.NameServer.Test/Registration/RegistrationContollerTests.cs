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
        private readonly IWorkContext _workContext = WorkContext.Empty;

        public RegistrationContollerTests(TestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task WhenNodeRegistrating_GivenTestNode_ShouldRegister()
        {
            await _fixture.NameServerClient.ClearAll(_workContext);

            var request = new RouteRegistrationRequest { NodeId = "test/Node1" };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var httpResponse = await _fixture.Client
                .PostAsync("api/Registgration", content);

            httpResponse.EnsureSuccessStatusCode();

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            RouteRegistrationResponse response = JsonConvert.DeserializeObject<RouteRegistrationResponse>(responseString);

            response.InputQueueUri.Should().Be("test/Node1");
        }

        [Fact]
        public async Task WhenNodeRegistrating_GivenRestClient_ShouldRegister()
        {
            await _fixture.NameServerClient.ClearAll(_workContext);

            var request = new RouteRegistrationRequest { NodeId = "test/Node1" };

            RouteRegistrationResponse response = await _fixture.NameServerClient.Register(_workContext, request);

            response.InputQueueUri.Should().Be("test/Node1");
        }

        [Fact]
        public async Task WhenNodeNotExist_GivenLookup_ShouldFail()
        {
            await _fixture.NameServerClient.ClearAll(_workContext);

            var request = new RouteLookupRequest { NodeId = "test/Node1" };

            RouteLookupResponse? response = await _fixture.NameServerClient.Lookup(_workContext, request);
            response.Should().BeNull();
        }

        [Fact]
        public async Task WhenNodeRegistered_GivenUnregister_ShouldPass()
        {
            await _fixture.NameServerClient.ClearAll(_workContext);

            var request = new RouteRegistrationRequest { NodeId = "test/Node1" };

            RouteRegistrationResponse response = await _fixture.NameServerClient.Register(_workContext, request);

            var lookupRequest = new RouteLookupRequest { NodeId = request.NodeId };

            RouteLookupResponse? lookupResponse = await _fixture.NameServerClient.Lookup(_workContext, lookupRequest);
            lookupResponse.Should().NotBeNull();
            lookupResponse!.NodeId.Should().Be(request.NodeId);

            await _fixture.NameServerClient.Unregister(_workContext, request);

            lookupResponse = await _fixture.NameServerClient.Lookup(_workContext, lookupRequest);
            lookupResponse.Should().BeNull();
        }
    }
}
