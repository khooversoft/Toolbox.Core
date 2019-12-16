// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public static class RestClientExtensions
    {
        public static async Task<T> GetContentAsync<T>(this Task<HttpResponseMessage> message, IWorkContext context)
            where T : class
        {
            RestResponse<T> response = await message.GetResponseAsync<T>(context);
            return response.Content;
        }

        public static async Task<T> GetContentAsync<T>(this HttpResponseMessage message, IWorkContext context)
            where T : class
        {
            RestResponse<T> response = await message.GetResponseAsync<T>(context);
            return response.Content;
        }

        public static async Task<RestResponse> GetResponseAsync(this Task<HttpResponseMessage> message, IWorkContext context)
        {
            HttpResponseMessage restResponse = await message;
            return new RestResponse(restResponse);
        }

        public static async Task<RestResponse<T>> GetResponseAsync<T>(this Task<HttpResponseMessage> message, IWorkContext context)
            where T : class
        {
            HttpResponseMessage restResponse = await message;
            return await GetResponseAsync<T>(restResponse, context);
        }

        public static async Task<RestResponse<T>> GetResponseAsync<T>(this HttpResponseMessage message, IWorkContext context)
            where T : class
        {
            context.Verify(nameof(context)).IsNotNull();
            context = context.WithMethodName();

            try
            {
                string json = await message.Content.ReadAsStringAsync();

                if (typeof(T) == typeof(string))
                {
                    return new RestResponse<T>(message, (T)(object)json);
                }

                return new RestResponse<T>(message, JsonSerializer.Deserialize<T>(json));
            }
            catch (Exception ex)
            {
                context.Telemetry.Error(context, $"{nameof(GetResponseAsync)} error '{ex.Message}", ex);
                throw;
            }
        }
    }
}
