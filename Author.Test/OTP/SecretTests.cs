using System;
using Author.OTP;
using System.Security.Cryptography;
using Xunit;
using Type = Author.OTP.Type;

namespace Author.Test.OTP
{
    public class SecretTests
    {
        [Fact]
        public void TestParseWithValidUri()
        {
            Secret reference = new Secret
            {
                Identifier = Guid.Parse("ac9fe03c-e1a1-47ff-b542-58b0973653bb"),
                Type = Type.Time,
                Name = "john.doe@email.com",
                Data = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
                Issuer = "ACME Co",
                Algorithm = HashAlgorithmName.SHA1,
                Digits = 6,
                Counter = 0,
                Period = 30
            };
            string uri = "otpauth://totp/ACME%20Co:john.doe@email.com?" +
                         "secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&" +
                         "issuer=ACME%20Co&algorithm=SHA1&digits=6&" +
                         "uuid=ac9fe03c-e1a1-47ff-b542-58b0973653bb&" +
                         "period=30";
            Secret parsed = Secret.Parse(uri);
            Assert.Equal(reference, parsed);
        }

        [Fact]
        public void TestToStringWithValidSecret()
        {
            string uri = "otpauth://totp/ACME%20Co:john.doe@email.com?" +
                         "secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&" +
                         "issuer=ACME%20Co&algorithm=SHA1&digits=6&" +
                         "uuid=ac9fe03c-e1a1-47ff-b542-58b0973653bb&" +
                         "period=30";
            var result = new Secret
            {
                Identifier = Guid.Parse("ac9fe03c-e1a1-47ff-b542-58b0973653bb"),
                Type = Type.Time,
                Name = "john.doe@email.com",
                Data = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
                Issuer = "ACME Co",
                Algorithm = HashAlgorithmName.SHA1,
                Digits = 6,
                Counter = 0,
                Period = 30
            }.ToString();
            Assert.Equal(uri, result);
        }
    }
}
