namespace Author.OTP
{
    public sealed class BlizzardApp : TimeBased
    {
        public const byte DefaultDigits = 8;
        public const byte DefaultPeriod = 30;

        public BlizzardApp(string secret)
            : base(secret)
        { }

        public override string GetCode(long timestamp, byte digits, byte period)
        {
            return base.GetCode(timestamp, DefaultDigits, DefaultPeriod);
        }
    }
}
