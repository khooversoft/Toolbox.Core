using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// REST Client provides a builder pattern for making a REST API call.
    /// 
    /// Note: if the Absolute URI is specified, this URI is used and not build
    /// process is not used.
    /// </summary>
    public class RestClient
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Create REST client and use provided HttpClient
        /// </summary>
        /// <param name="client"></param>
        public RestClient(HttpClient client)
        {
            client.Verify(nameof(client)).IsNotNull();

            _client = client;
        }

        /// <summary>
        /// Base URI
        /// </summary>
        public Uri? BaseAddress { get; private set; }

        /// <summary>
        /// Path elements
        /// </summary>
        public StringVector PathItems { get; private set; } = StringVector.Empty;

        /// <summary>
        /// Query items
        /// </summary>
        public IDictionary<string, string> QueryItems { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Content for the REST API call
        /// </summary>
        public HttpContent? Content { get; private set; }

        /// <summary>
        /// Insure success status code (any 200)
        /// </summary>
        public bool EnsureSuccessStatusCode { get; private set; }

        /// <summary>
        /// Valid HTTP Status codes
        /// </summary>
        public HttpStatusCode[] ValidHttpStatusCodes { get; private set; } = Array.Empty<HttpStatusCode>();

        /// <summary>
        /// Clear all settings
        /// </summary>
        /// <returns></returns>
        public RestClient Clear()
        {
            QueryItems.Clear();
            PathItems = StringVector.Empty;
            BaseAddress = null;

            return this;
        }

        /// <summary>
        /// Set base address of the REST URI
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns>this</returns>
        public RestClient SetBaseAddress(Uri baseAddress)
        {
            baseAddress.Verify(nameof(baseAddress)).IsNotNull();

            var build = new UriBuilder(baseAddress);
            build.Path = null;
            build.Query = null;

            BaseAddress = build.Uri;
            return this;
        }

        /// <summary>
        /// Add path item (string vector(s))
        /// </summary>
        /// <param name="values">path elements</param>
        /// <returns>this</returns>
        public RestClient AddPath(params string[] values)
        {
            values.Verify(nameof(values)).IsNotNull();

            PathItems = PathItems.With(values);

            return this;
        }

        /// <summary>
        /// Add query item
        /// </summary>
        /// <param name="name">name of query</param>
        /// <param name="value">value of query</param>
        /// <returns>this</returns>
        public RestClient AddQuery(string name, string value)
        {
            name.Verify(nameof(name)).IsNotEmpty();
            value.Verify(nameof(value)).IsNotEmpty();

            QueryItems.Add(name, value);

            return this;
        }

        /// <summary>
        /// Set content
        /// </summary>
        /// <param name="content">HTTP content</param>
        /// <returns>this</returns>
        public RestClient SetContent(HttpContent content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        /// Set content
        /// </summary>
        /// <typeparam name="T">type to serialize</typeparam>
        /// <param name="value">value type instance</param>
        /// <param name="required">true if required, false return with out setting</param>
        /// <returns>this</returns>
        public RestClient SetContent<T>(T value, bool required = true)
        {
            value.Verify(nameof(value)).Assert(x => !required || x != null, "Value is required but null");

            string jsonString = JsonConvert.SerializeObject(value);
            Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return this;
        }

        /// <summary>
        /// Set ensure success status code (default false)
        /// </summary>
        /// <param name="ensureSuccessStatusCode">true or false (default true)</param>
        /// <returns>this</returns>
        public RestClient SetEnsureSuccessStatusCode(bool ensureSuccessStatusCode = true)
        {
            EnsureSuccessStatusCode = ensureSuccessStatusCode;
            return this;
        }

        /// <summary>
        /// Set valid HTTP status codes
        /// </summary>
        /// <param name="httpStatusCodes"></param>
        /// <returns></returns>
        public RestClient SetValidHttpStatusCodes(params HttpStatusCode[] httpStatusCodes)
        {
            httpStatusCodes.Verify(nameof(httpStatusCodes)).IsNotNull();

            ValidHttpStatusCodes = httpStatusCodes;
            return this;
        }

        /// <summary>
        /// Issue Get
        /// </summary>
        /// <param name="context">work context</param>
        /// <returns>this</returns>
        public Task<HttpResponseMessage> GetAsync(IWorkContext context)
        {
            return SendAsync(context, BuildRequestMessage(HttpMethod.Get));
        }

        /// <summary>
        /// Issue Delete
        /// </summary>
        /// <param name="context">work context</param>
        /// <returns>this</returns>
        public Task<HttpResponseMessage> DeleteAsync(IWorkContext context)
        {
            return SendAsync(context, BuildRequestMessage(HttpMethod.Delete));
        }

        /// <summary>
        /// Issue Post
        /// </summary>
        /// <param name="context">work context</param>
        /// <returns>this</returns>
        public Task<HttpResponseMessage> PostAsync(IWorkContext context)
        {
            return SendAsync(context, BuildRequestMessage(HttpMethod.Post));
        }

        /// <summary>
        /// Issue Put
        /// </summary>
        /// <param name="context">work context</param>
        /// <returns>this</returns>
        public Task<HttpResponseMessage> PutAsync(IWorkContext context)
        {
            return SendAsync(context, BuildRequestMessage(HttpMethod.Put));
        }

        /// <summary>
        /// Send request
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="requestMessage">request message</param>
        /// <returns>state of HTTP response</returns>
        private async Task<HttpResponseMessage> SendAsync(IWorkContext context, HttpRequestMessage requestMessage)
        {
            context.Verify(nameof(context)).IsNotNull();
            requestMessage.Verify(nameof(requestMessage)).IsNotNull();

            context = context.WithMethodName();

            try
            {
                using var scope = new TelemetryActivityScope(context, requestMessage.RequestUri.ToString());
                HttpResponseMessage message = await _client.SendAsync(requestMessage, context.CancellationToken);

                if (!ValidHttpStatusCodes.Any(x => message.StatusCode == x))
                {
                    if (EnsureSuccessStatusCode) message.EnsureSuccessStatusCode();
                }

                return message;
            }
            catch (Exception ex)
            {
                context.Telemetry.Error(context, $"{nameof(SendAsync)} error '{ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Build HTTP request
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <returns>HTTP request message</returns>
        private HttpRequestMessage BuildRequestMessage(HttpMethod method)
        {
            var builder = BaseAddress != null ? new UriBuilder(BaseAddress) : new UriBuilder()
            {
                Path = PathItems
            };

            if (QueryItems.Count > 0)
            {
                builder.Query = string.Join("&", QueryItems.Select(x => Uri.EscapeDataString(x.Key).Trim() + "=" + Uri.EscapeDataString(x.Value).Trim()));
            }

            var request = new HttpRequestMessage(method, builder.Uri)
            {
                Content = Content,
            };

            return request;
        }
    }
}
