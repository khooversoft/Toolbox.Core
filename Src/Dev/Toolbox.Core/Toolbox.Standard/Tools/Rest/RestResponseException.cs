using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    [Serializable]
    public class RestReponseException : Exception
    {
        public RestReponseException()
        {
        }

        public RestReponseException(string message)
            : base(message)
        {
        }

        public RestReponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RestReponseException(string message, int httpCode) : base(message)
        {
            HttpCode = httpCode;
        }

        public RestReponseException(string message, int httpCode, Exception inner)
            : base(message, inner)
        {
            HttpCode = httpCode;
        }

        protected RestReponseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }


        public int HttpCode { get; }
    }
}
