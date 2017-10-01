using System;

namespace Toffee.Util
{
    public static class TimeUtil
    {
        public static long GetUnixTimestamp()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
