namespace Author.OTP
{
    public sealed class Authy : TimeBased
    {
        public const byte DefaultDigits = 7;
        public const byte DefaultPeriod = 10;

        public Authy(Secret secret)
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
