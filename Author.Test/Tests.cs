using System;
using Xunit;
using Author.Utility;

namespace Author.Test
{
    public class Tests
    {
        [Fact]
        public void OtpAuthTest()
        {
            var uri = new Uri("otpauth://totp/ACME%20Co:john.doe@email.com?" +
                              "secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&" +
                              "issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30");
            Assert.Equal(OtpAuth.ParseString(uri), new OtpAuth
            {
                Type = OTP.Type.Time,
                Name = "john.doe@email.com",
                Secret = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
                Issuer = "ACME Co",
                Algorithm = "SHA1",
                Digits = 6,
                Counter = 0,
                Period = 30
            });
        }
    }
}
