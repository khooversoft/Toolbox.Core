using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class DataBlockModel<T>
    {
        public DateTime TimeStamp { get; set; }

        public string BlockType { get; set; }

        public string BlockId { get; set; }

        public T Data { get; set; }
    }
}
