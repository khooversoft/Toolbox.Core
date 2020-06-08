using System;

namespace Microservice.Interface.Test
{
    public class CustomerInfoResponse
    {
        public string? CustomerId { get; set; }

        public string? Addr1 { get; set; }
        public string? Addr2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
    }
}
