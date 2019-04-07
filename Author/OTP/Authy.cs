namespace Author.OTP
{
    public sealed class Authy : TimeBased
    {
        public const byte DefaultDigits = 7;
        public const byte DefaultPeriod = 10;

        public Authy(string secret)
            : base(secret, DefaultAlgorithm)
        { }

        public override string GetCode(long timestamp, byte digits, byte period)
        {
            return base.GetCode(timestamp, DefaultDigits, DefaultPeriod);
        }
    }
}
