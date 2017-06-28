namespace Author.OTP
{
    public interface IBaseGenerator
    {
        string Type { get; set; }

        string Name { get; set; }

        byte Digits { get; set; }

        byte Period { get; set; }

        string SecretData { get; set; }

        string GetCode(long counter);
    }
}
