// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public static class RestClientExtensions
    {
        public static async Task<T> GetContentAsync<T>(this Task<HttpResponseMessage> message)
            where T : class
        {
            RestResponse<T> response = await message.GetResponseAsync<T>();
            return response.Content;
        }

        public static async Task<T> GetContentAsync<T>(this HttpResponseMessage message)
            where T : class
        {
            RestResponse<T> response = await message.GetResponseAsync<T>();
            return response.Content;
        }

        public static async Task<RestResponse> GetResponseAsync(this Task<HttpResponseMessage> message)
        {
            HttpResponseMessage restResponse = await message;
            return new RestResponse(restResponse);
        }

        public static async Task<RestResponse<T>> GetResponseAsync<T>(this Task<HttpResponseMessage> message)
            where T : class
        {
            HttpResponseMessage restResponse = await message;
            return await GetResponseAsync<T>(restResponse);
        }

        public static async Task<RestResponse<T>> GetResponseAsync<T>(this HttpResponseMessage message)
            where T : class
        {
            string json = await message.Content.ReadAsStringAsync();

            if (typeof(T) == typeof(string))
            {
                return new RestResponse<T>(message, (T)(object)json);
            }

            T returnType = JsonConvert.DeserializeObject<T>(json);
            return new RestResponse<T>(message, returnType);
        }
    }
}
