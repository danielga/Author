using System;
using System.Collections.Generic;

namespace Author.OTP
{
    public struct Type : IEquatable<Type>
    {
        public string Name { get; private set; }
        public int Value { get; private set; }

        public static Type Hash { get; private set; } = new Type("hotp", 0);
        public static Type Time { get; private set; } = new Type("totp", 1);
        public static Type Steam { get; private set; } = new Type("steam", 2);
        public static Type Blizzard { get; private set; } = new Type("blizzard", 3);
        public static Type Authy { get; private set; } = new Type("authy", 4);

        private static readonly Dictionary<string, Type> FromName = new Dictionary<string, Type>
        {
            { Hash.Name, Hash },
            { Time.Name, Time },
            { Steam.Name, Steam },
            { Blizzard.Name, Blizzard },
            { Authy.Name, Authy }
        };

        private static readonly Type[] FromValue = new Type[] { Hash, Time, Steam, Blizzard, Authy };

        private Type(string name, int value)
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

        public static Type Parse(int input)
        {
            if (input < 0 || input > FromValue.Length)
            {
                throw new ArgumentException("Invalid type value", nameof(input));
            }

            return FromValue[input];
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
