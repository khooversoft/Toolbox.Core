using System.Collections.Generic;

namespace Khooversoft.Toolbox.BlockDocument
{
    public interface IBlockData
    {
        IReadOnlyList<byte> GetUTF8Bytes();
    }
}