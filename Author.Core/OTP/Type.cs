namespace Author.OTP
{
    public enum Type
    {
        Hash,
        Time,
        Steam,
        Blizzard,
        Authy
    }

    public static class TypeExtensions
    {
        private static readonly Dictionary<string, Type> FromNameMap = new()
        {
            { "hotp", Type.Hash },
            { "totp", Type.Time },
            { "steam", Type.Steam },
            { "blizzard", Type.Blizzard },
            { "authy", Type.Authy }
        };

        public static Type Parse(string? name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!FromNameMap.TryGetValue(name, out var value))
                throw new ArgumentException("Invalid type name", nameof(name));

            return value;
        }

        public static string ToString(Type? value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value switch
            {
                Type.Hash => "hotp",
                Type.Time => "totp",
                Type.Steam => "steam",
                Type.Blizzard => "blizzard",
                Type.Authy => "authy",
                _ => throw new ArgumentException("Invalid type value", nameof(value)),
            };
        }
    }
}
