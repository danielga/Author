using System;

namespace Author.OTP
{
    public struct Secret
    {
        public Guid Identifier;
        public byte Type;
        public string Name;
        public byte Digits;
        public byte Period;
        public string Data;
    }
}
