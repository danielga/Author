namespace Author.OTP
{
    public interface IBaseGenerator
    {
        string GetCode(long counter, byte digits, byte period);
    }
}
