using System.Security.Cryptography;

namespace Author.OTP
{
    public class TimeBased : HashBased
    {
        public TimeBased(string secret, HashAlgorithmName algorithm)
            : base(secret, algorithm)
        { }

        public override string GetCode(long timestamp, byte digits, byte period)
        {
            return base.GetCode(timestamp / period, digits, period);
        }
    }
}
