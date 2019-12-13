using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public static class SerializationConvertToExtensions
    {
        public static DataBlockModel<T> ConvertTo<T>(this DataBlock<T> subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new DataBlockModel<T>
            {
                TimeStamp = subject.TimeStamp,
                BlockType = subject.BlockType,
                BlockId = subject.BlockId,
                Data = subject.Data,
            };
        }

        public static DataBlock<T> ConvertTo<T>(this DataBlockModel<T> subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new DataBlock<T>(subject.TimeStamp, subject.BlockType, subject.BlockId, subject.Data);
        }
    }
}
