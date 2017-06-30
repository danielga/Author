using System.Collections.Generic;

namespace Author.OTP
{
    public static class Type
    {
        public const byte Hash = 0;
        public const byte Time = 1;
        public const byte Steam = 2;
        public const byte Blizzard = 3;
        public const byte Authy = 4;

        public static readonly Dictionary<string, byte> FromName = new Dictionary<string, byte>
        {
            { "hash", Hash },
            { "time", Time },
            { "steam", Steam },
            { "blizzard", Blizzard },
            { "authy", Authy }
        };

        public static readonly Dictionary<byte, string> Name = new Dictionary<byte, string>
        {
            { Hash, "hash" },
            { Time, "time" },
            { Steam, "steam" },
            { Blizzard, "blizzard" },
            { Authy, "authy" }
        };
    }
}
