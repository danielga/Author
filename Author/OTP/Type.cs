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
        public const byte Maximum = Authy;

        public static readonly Dictionary<string, byte> FromName = new Dictionary<string, byte>
        {
            { "hotp", Hash },
            { "totp", Time },
            { "steam", Steam },
            { "blizzard", Blizzard },
            { "authy", Authy }
        };

        public static readonly Dictionary<byte, string> Name = new Dictionary<byte, string>
        {
            { Hash, "hotp" },
            { Time, "totp" },
            { Steam, "steam" },
            { Blizzard, "blizzard" },
            { Authy, "authy" }
        };
    }
}
