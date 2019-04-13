using System;
using System.Collections.Generic;

namespace Author.OTP
{
    public struct Type : IEquatable<Type>
    {
        private enum ValueEnum
        {
            Hash = 0,
            Time = 1,
            Steam = 2,
            Blizzard = 3,
            Authy = 4,
            Maximum = Authy
        }

        public string Name { get; private set; }
        private readonly ValueEnum Value;

        public static readonly Type Hash = new Type("hotp", ValueEnum.Hash);
        public static readonly Type Time = new Type("totp", ValueEnum.Time);
        public static readonly Type Steam = new Type("steam", ValueEnum.Steam);
        public static readonly Type Blizzard = new Type("blizzard", ValueEnum.Blizzard);
        public static readonly Type Authy = new Type("authy", ValueEnum.Authy);

        private static readonly Dictionary<string, Type> FromName = new Dictionary<string, Type>
        {
            { Hash.Name, Hash },
            { Time.Name, Time },
            { Steam.Name, Steam },
            { Blizzard.Name, Blizzard },
            { Authy.Name, Authy }
        };

        private Type(string name, ValueEnum value)
        {
            Name = name;
            Value = value;
        }

        public bool Equals(Type other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Type && Equals((Type)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Type lhs, Type rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Type lhs, Type rhs)
        {
            return !(lhs == rhs);
        }

        public static Type Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (!FromName.TryGetValue(input, out Type value))
            {
                throw new ArgumentException("Invalid type name", nameof(input));
            }

            return value;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
