using System.Security.Cryptography;

namespace Author.OTP
{
    public static class Factory
    {
        private static readonly Dictionary<Type, Func<string, HashAlgorithmName, IBaseGenerator>> Constructors = new()
        {
            {Type.Hash, (secret, algo) => new HashBased(secret, algo)},
            {Type.Time, (secret, algo) => new TimeBased(secret, algo)},
            {Type.Steam, (secret, _) => new Steam(secret)},
            {Type.Blizzard, (secret, _) => new BlizzardApp(secret)},
            {Type.Authy, (secret, _) => new Authy(secret)}
        };

        public static IBaseGenerator CreateGenerator(Type type, string secret, HashAlgorithmName algo)
        {
            if (!Constructors.TryGetValue(type, out var constructor))
                throw new ArgumentException("Invalid type", nameof(type));

            return constructor(secret, algo);
        }
    }
}
