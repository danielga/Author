using System.Collections.Generic;

namespace Author.OTP
{
    public static class Type
    {
        public const int Hash = 0;
        public const int Time = 1;
        public const int Steam = 2;
        public const int Blizzard = 3;
        public const int Authy = 4;

        public static readonly Dictionary<string, int> FromName = new Dictionary<string, int>
        {
            { "hash", Hash },
            { "time", Time },
            { "steam", Steam },
            { "blizzard", Blizzard },
            { "authy", Authy }
        };

        public static readonly Dictionary<int, string> Name = new Dictionary<int, string>
        {
            { Hash, "hash" },
            { Time, "time" },
            { Steam, "steam" },
            { Blizzard, "blizzard" },
            { Authy, "authy" }
        };
    }
}
