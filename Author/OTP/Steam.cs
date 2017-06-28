using System;
using System.Text;

namespace Author.OTP
{
    public sealed class Steam : TimeBased
    {
        public const byte DefaultDigits = 5;
        public const byte DefaultPeriod = 30;

        static readonly char[] Characters =
        {
            '2', '3', '4', '5', '6', '7', '8', '9', 'B', 'C',
            'D', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q',
            'R', 'T', 'V', 'W', 'X', 'Y'
        };

        public Steam(Secret secret)
            : base(
                new Secret
                {
                    Name = secret.Name,
                    Type = secret.Type,
                    Digits = DefaultDigits,
                    Period = DefaultPeriod,
                    Data = secret.Data
                })
        { }

        public override string GetCode(long timestamp)
        {
            byte[] ts = BitConverter.GetBytes(timestamp / Period);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(ts);

            Hash.Append(ts);
            byte[] mac = Hash.GetValueAndReset();

            int start = mac[19] & 0x0f;
            byte[] bytes = new byte[4];
            Array.Copy(mac, start, bytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            uint fullcode = BitConverter.ToUInt32(bytes, 0) & 0x7fffffff;
            StringBuilder code = new StringBuilder();
            for (int i = 0; i < Digits; ++i)
            {
                code.Append(Characters[fullcode % Characters.Length]);
                fullcode /= (uint)Characters.Length;
            }

            return code.ToString();
        }
    }
}
