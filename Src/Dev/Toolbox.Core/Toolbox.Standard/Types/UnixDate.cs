using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Unix date is number of seconds from 1970-01-01T00:00:00Z
    /// </summary>
    public struct UnixDate
    {
        public UnixDate(long timeStamp)
        {
            TimeStamp = timeStamp;
        }

        public UnixDate(DateTimeOffset utcDate)
        {
            TimeStamp = utcDate.ToUnixTimeSeconds();
        }

        public long TimeStamp { get; }

        public static UnixDate UtcNow => (UnixDate)DateTimeOffset.UtcNow;

        public static implicit operator DateTimeOffset(UnixDate unixDate)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixDate.TimeStamp);
        }

        public static implicit operator long(UnixDate unix)
        {
            return unix.TimeStamp;
        }

        public static explicit operator UnixDate(long timeStamp)
        {
            return new UnixDate(timeStamp);
        }

        public static explicit operator UnixDate(DateTimeOffset dateTimeOffset)
        {
            return new UnixDate(dateTimeOffset);
        }
    }

    public static class UnixDateExtensions
    {
        public static UnixDate ToUnixDate(this long value)
        {
            return new UnixDate(value);
        }
    }
}
