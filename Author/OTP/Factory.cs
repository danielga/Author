using System;
using System.Collections.Generic;

namespace Author.OTP
{
    public static class Factory
    {
        static readonly Dictionary<string, Func<string, IBaseGenerator>> Constructors = new Dictionary
            <string, Func<string, IBaseGenerator>>
        {
            {"hash", secret => new HashBased(secret)},
            {"time", secret => new TimeBased(secret)},
            {"steam", secret => new Steam(secret)},
            {"blizzard", secret => new BlizzardApp(secret)},
            {"authy", secret => new Authy(secret)}
        };

        public static IBaseGenerator CreateGenerator(string type, string secret)
        {
            Func<string, IBaseGenerator> constructor;
            return Constructors.TryGetValue(type.ToLowerInvariant(), out constructor)
                ? constructor(secret)
                : null;
        }
    }
}
