using System;
using System.Collections.Generic;

namespace Author.OTP
{
    public static class Factory
    {
        static readonly Dictionary<string, Func<Secret, IBaseGenerator>> Constructors = new Dictionary
            <string, Func<Secret, IBaseGenerator>>
        {
            {"hash", secret => new HashBased(secret)},
            {"time", secret => new TimeBased(secret)},
            {"steam", secret => new Steam(secret)},
            {"blizzard", secret => new BlizzardApp(secret)},
            {"authy", secret => new Authy(secret)}
        };

        public static IBaseGenerator CreateGenerator(Secret secret)
        {
            Func<Secret, IBaseGenerator> constructor;
            return Constructors.TryGetValue(secret.Type.ToLowerInvariant(), out constructor)
                ? constructor(secret)
                : null;
        }
    }
}
