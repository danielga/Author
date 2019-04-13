using System;
using System.Collections.Generic;

namespace Author.OTP
{
    public static class Factory
    {
        private static readonly Dictionary<Type, Func<string, IBaseGenerator>> Constructors =
            new Dictionary<Type, Func<string, IBaseGenerator>>
        {
            {Type.Hash, secret => new HashBased(secret, HashBased.DefaultAlgorithm)},
            {Type.Time, secret => new TimeBased(secret, HashBased.DefaultAlgorithm)},
            {Type.Steam, secret => new Steam(secret)},
            {Type.Blizzard, secret => new BlizzardApp(secret)},
            {Type.Authy, secret => new Authy(secret)}
        };

        public static IBaseGenerator CreateGenerator(Type type, string secret)
        {
            return Constructors.TryGetValue(type, out var constructor) ?
                constructor(secret) :
                null;
        }
    }
}
