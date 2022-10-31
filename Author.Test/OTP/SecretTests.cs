using Author.OTP;
using System.Security.Cryptography;
using Type = Author.OTP.Type;

namespace Author.Test.OTP
{
    [TestClass]
    public class SecretTests
    {
        [TestMethod]
        public void TestParseWithValidUri()
        {
            var reference = new Secret("HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ")
            {
                Identifier = Guid.Parse("ac9fe03c-e1a1-47ff-b542-58b0973653bb"),
                Type = Type.Time,
                Name = "john.doe@email.com",
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
            var parsed = Secret.Parse(uri);
            Assert.AreEqual(reference, parsed);
        }

        [TestMethod]
        public void TestToStringWithValidSecret()
        {
            string uri = "otpauth://totp/ACME%20Co:john.doe@email.com?" +
                         "secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&" +
                         "issuer=ACME%20Co&algorithm=SHA1&digits=6&" +
                         "uuid=ac9fe03c-e1a1-47ff-b542-58b0973653bb&" +
                         "period=30";
            var result = new Secret("HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ")
            {
                Identifier = Guid.Parse("ac9fe03c-e1a1-47ff-b542-58b0973653bb"),
                Type = Type.Time,
                Name = "john.doe@email.com",
                Issuer = "ACME Co",
                Algorithm = HashAlgorithmName.SHA1,
                Digits = 6,
                Counter = 0,
                Period = 30
            }.ToString();
            Assert.AreEqual(uri, result);
        }
    }
}
