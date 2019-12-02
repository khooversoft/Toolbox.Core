using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class RestResponse
    {
        public RestResponse(HttpResponseMessage httpResponseMessage)
        {
            HttpResponseMessage = httpResponseMessage;
        }

        public HttpResponseMessage HttpResponseMessage { get; }

    }

    public class RestResponse<T> : RestResponse where T : class
    {
        public RestResponse(HttpResponseMessage httpResponseMessage, T content)
            :base(httpResponseMessage)
        {
            Content = content;
        }

        public T Content { get; }
    }
}
