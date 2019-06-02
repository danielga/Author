using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Author.OTP
{
    public static class Factory
    {
        private static readonly Dictionary<Type, Func<string, HashAlgorithmName, IBaseGenerator>> Constructors =
            new Dictionary<Type, Func<string, HashAlgorithmName, IBaseGenerator>>
        {
            {Type.Hash, (secret, algo) => new HashBased(secret, algo)},
            {Type.Time, (secret, algo) => new TimeBased(secret, algo)},
            {Type.Steam, (secret, _) => new Steam(secret)},
            {Type.Blizzard, (secret, _) => new BlizzardApp(secret)},
            {Type.Authy, (secret, _) => new Authy(secret)}
        };

        public static IBaseGenerator CreateGenerator(Type type, string secret, HashAlgorithmName algo)
        {
            return Constructors.TryGetValue(type, out var constructor) ?
                constructor(secret, algo) :
                null;
        }
    }
}
