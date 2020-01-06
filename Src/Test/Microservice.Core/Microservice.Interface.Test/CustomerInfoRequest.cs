using System;

namespace Microservice.Interface.Test
{
    public class CustomerInfoRequest
    {
        public string? CustomerId { get; set; }

        public bool RequestAddress { get; set; }
    }
}
