using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class MessageActivityModel
    {
        public Guid ActivityId { get; set; }

        public Guid? ParentActivityId { get; set; }
    }
}
