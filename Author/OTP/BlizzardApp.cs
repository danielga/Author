namespace Author.OTP
{
    public sealed class BlizzardApp : TimeBased
    {
        public const byte DefaultDigits = 8;
        public const byte DefaultPeriod = 30;

        public BlizzardApp(Secret secret)
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
    }
}
