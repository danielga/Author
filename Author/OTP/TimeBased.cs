using PCLCrypto;

namespace Author.OTP
{
    public class TimeBased : HashBased
    {
        public TimeBased(Secret secret, MacAlgorithm algorithm = MacAlgorithm.HmacSha1)
            : base(secret, algorithm)
        {
            Period = secret.Period;
        }

        public override string GetCode(long timestamp)
        {
            return base.GetCode(timestamp / Period);
        }
    }
}
