using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Azure
{
    public class DatalakePathProperties
    {
        public DateTimeOffset LastModified { get; set; }

        public string? ContentEncoding { get; set; }

        public string? ETag { get; set; }

        public string? ContentType { get; set; }

        public long ContentLength { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
