using PCLCrypto;

namespace Author.OTP
{
    public class TimeBased : HashBased
    {
        public TimeBased(string secret, MacAlgorithm algorithm = MacAlgorithm.HmacSha1)
            : base(secret, algorithm)
        { }

        public override string GetCode(long timestamp, byte digits, byte period)
        {
            return base.GetCode(timestamp / period, digits, period);
        }
    }
}
