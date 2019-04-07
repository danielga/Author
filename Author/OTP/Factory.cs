using System;
using System.Collections.Generic;

namespace Author.OTP
{
    public static class Factory
    {
        private static readonly Dictionary<byte, Func<string, IBaseGenerator>> Constructors =
            new Dictionary<byte, Func<string, IBaseGenerator>>
        {
            {Type.Hash, secret => new HashBased(secret, HashBased.DefaultAlgorithm)},
            {Type.Time, secret => new TimeBased(secret, HashBased.DefaultAlgorithm)},
            {Type.Steam, secret => new Steam(secret)},
            {Type.Blizzard, secret => new BlizzardApp(secret)},
            {Type.Authy, secret => new Authy(secret)}
        };

        public static IBaseGenerator CreateGenerator(byte type, string secret)
        {
            return Constructors.TryGetValue(type, out var constructor) ?
                constructor(secret) :
                null;
        }
    }
}
