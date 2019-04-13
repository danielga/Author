using Author.OTP;
using System.Security.Cryptography;
using Xunit;

namespace Author.Test.OTP
{
    public class SecretTests
    {
        [Fact]
        public void TestParseWithValidUri()
        {
            string uri = "otpauth://totp/ACME%20Co:john.doe@email.com?" +
                         "secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&" +
                         "issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30";
            Assert.Equal(new Secret
            {
                Type = Type.Time,
                Name = "john.doe@email.com",
                Data = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
                Issuer = "ACME Co",
                Algorithm = HashAlgorithmName.SHA1,
                Digits = 6,
                Counter = 0,
                Period = 30
            }, Secret.Parse(uri));
        }

        [Fact]
        public void TestToStringWithValidSecret()
        {
            string uri = "otpauth://totp/ACME%20Co:john.doe@email.com?" +
                         "secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&" +
                         "issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30";
            Assert.Equal(uri, new Secret
            {
                Type = Type.Time,
                Name = "john.doe@email.com",
                Data = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
                Issuer = "ACME Co",
                Algorithm = HashAlgorithmName.SHA1,
                Digits = 6,
                Counter = 0,
                Period = 30
            }.ToString());
        }
    }
}
