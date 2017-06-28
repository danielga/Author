using System;

namespace Author.Utility
{
    public static class Time
    {
        static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrent()
        {
            return (long)(DateTime.UtcNow - Epoch).TotalSeconds;
        }
    }
}
