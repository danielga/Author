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

        public Steam(string secret)
            : base(secret, DefaultAlgorithm)
        { }

        public override string GetCode(long timestamp, byte digits, byte period)
        {
            byte[] ts = BitConverter.GetBytes(timestamp / DefaultPeriod);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(ts);

            Hash.AppendData(ts);
            byte[] mac = Hash.GetHashAndReset();

            int start = mac[19] & 0x0f;
            byte[] bytes = new byte[4];
            Array.Copy(mac, start, bytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            uint fullcode = BitConverter.ToUInt32(bytes, 0) & 0x7fffffff;
            StringBuilder code = new();
            for (int i = 0; i < DefaultDigits; ++i)
            {
                code.Append(Characters[fullcode % Characters.Length]);
                fullcode /= (uint)Characters.Length;
            }

            return code.ToString();
        }
    }
}
