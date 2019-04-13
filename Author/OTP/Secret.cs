using System;

namespace Author.OTP
{
    public struct Secret
    {
        public Guid Identifier;
        public Type Type;
        public string Name;
        public byte Digits;
        public byte Period;
        public string Data;
    }
}
