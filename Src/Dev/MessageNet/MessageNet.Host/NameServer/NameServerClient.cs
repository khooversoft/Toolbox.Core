// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Khooversoft.MessageNet.Host
{
    public class NameServerClient : INameServerClient
    {
        private readonly Uri? _nameServiceUri;
        private readonly HttpClient _httpClient;

        public NameServerClient(HttpClient httpClient)
        {
            httpClient.Verify(nameof(httpClient)).IsNotNull();

            _httpClient = httpClient;
        }

        public NameServerClient(Uri nameServiceUri, HttpClient httpClient)
        {
            nameServiceUri.Verify(nameof(nameServiceUri)).IsNotNull();
            httpClient.Verify(nameof(httpClient)).IsNotNull();

            _nameServiceUri = nameServiceUri;
            _httpClient = httpClient;
        }

        public async Task<bool> Ping(IWorkContext context)
        {
            string response = await new RestClient(_httpClient)
                .SetBaseAddress(_nameServiceUri)
                .AddPath("api/Ping")
                .SetEnsureSuccessStatusCode()
                .GetAsync(context)
                .GetContentAsync<string>(context);

            JObject jobj = JObject.Parse(response);
            return (bool)jobj["ok"]! == true;
        }

        public Task<RouteRegistrationResponse> Register(IWorkContext context, RouteRegistrationRequest request)
        {
            return new RestClient(_httpClient)
                .SetBaseAddress(_nameServiceUri)
                .AddPath("api/Registgration")
                .SetContent(request)
                .SetEnsureSuccessStatusCode()
                .PostAsync(context)
                .GetContentAsync<RouteRegistrationResponse>(context);
        }

        public Task Unregister(IWorkContext context, RouteRegistrationRequest request)
        {
            return new RestClient(_httpClient)
                .SetBaseAddress(_nameServiceUri)
                .AddPath("api/Registgration")
                .SetContent(request)
                .SetEnsureSuccessStatusCode()
                .DeleteAsync(context)
                .GetResponseAsync(context);
        }

        public async Task<RouteLookupResponse?> Lookup(IWorkContext context, RouteLookupRequest request)
        {
            RestResponse<RouteLookupResponse> response = await new RestClient(_httpClient)
                .SetBaseAddress(_nameServiceUri)
                .AddPath("api/Registgration")
                .SetContent(request)
                .SetValidHttpStatusCodes(HttpStatusCode.NotFound)
                .SetEnsureSuccessStatusCode()
                .GetAsync(context)
                .GetResponseAsync<RouteLookupResponse>(context);

            if (response.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound) return null;
            return response.Content;
        }

        public Task ClearAll(IWorkContext context)
        {
            return new RestClient(_httpClient)
                .SetBaseAddress(_nameServiceUri)
                .AddPath("api/Administration/clear")
                .SetEnsureSuccessStatusCode()
                .PostAsync(context);
        }
    }
}
